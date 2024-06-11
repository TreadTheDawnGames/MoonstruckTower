using Godot;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

public partial class EnemyV2 : Node2D
{
	public EnemyPathfinder pathfinder;
	public AnimatedSprite2D animator;
	public AnimatedSprite2D statusAnimator;
	public EnemyStateMachine machine;
	public Node2D flippables;
	public player link;
	RayCast2D visionCast;
	public bool canSee = false;
	public CollisionShape2D hurtBox;
	public CollisionShape2D collisionBox;
	public HitBox2D hitBox;
	public Area2D attackRangeArea;

	public bool isBusy = false;
	

	[Export]
	float sightRange = 160;
	[Export]
	int maxHP = 2;
	public int hitPoints;
	public Vector2 lastSighting;
	public Timer damageTimer;
	float damageWaitTime = 0.5f;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		hitPoints = maxHP;
		pathfinder = GetNode<EnemyPathfinder>(Owner.GetPath());
		collisionBox = GetNode<CollisionShape2D>(pathfinder.GetPath()+"/CollisionBox");
		animator = GetNode<AnimatedSprite2D>("Animator");
		statusAnimator = GetNode<AnimatedSprite2D>("StatusAnimator");
		machine = GetNode<EnemyStateMachine>("StateMachine");
		flippables = GetNode<Node2D>("Flippables");
		hurtBox = GetNode<CollisionShape2D>(flippables.GetPath()+"/HurtBox2D/HurtShape");
		visionCast = GetNode<RayCast2D>("VisionCast");
        hitBox = GetNode<HitBox2D>("Flippables/HitBox2D");
        attackRangeArea = GetNode<Area2D>("Flippables/AttackRangeFinder");
        damageTimer = GetNode<Timer>("DamageTimer");

        damageTimer.Timeout += () => pathfinder.takingDamage = false;

        link = (player)GetTree().GetFirstNodeInGroup("Player");

        machine.SetUp();
		attackRangeArea.BodyEntered += (node) => EnterAttackState(node);
	}

   

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		pathfinder.PathfinderProcess(delta);

		var relitiveLinkPos = link.GlobalPosition - GlobalPosition;

		if (GlobalPosition.DistanceTo(link.GlobalPosition) < sightRange)
			visionCast.TargetPosition = relitiveLinkPos;
		else
			visionCast.TargetPosition = Vector2.Zero;

		//facing right
        if (pathfinder.movementDirection > 0)
		{
			animator.FlipH = true;
			flippables.Scale = new Vector2(-1, 1);
			hitBox.damage = 1;
		}
		//facing left
        else if(pathfinder.movementDirection < 0) 
        {
			animator.FlipH = false;
            flippables.Scale = new Vector2(1, 1);
			hitBox.damage = -1;
        }

		if (visionCast.GetColliderRid() == link.GetRid())
		{	
			if(Mathf.Sign(relitiveLinkPos.X) >= 0 && animator.FlipH)
			{
                lastSighting = link.GlobalPosition;

                canSee = true;
			}
			else if(Mathf.Sign(relitiveLinkPos.X) <= 0 && !animator.FlipH)
			{
                lastSighting = link.GlobalPosition;

                canSee = true;
			}
			else
			{
				//GD.Print("Can't See");
                canSee = false;
			}
			if (machine.CurrentState != "EnemyChaseState" && canSee &&!isBusy)
			{

				//GD.Print("I SEE YOU");
				machine.ChangeState("EnemyChaseState", new Dictionary<string, object> { { "goToPoint", lastSighting } });
			}
			
			//if facing the direction of link


			

		}
		else
        {
            canSee = false;
		}
		if (Input.IsActionJustPressed("Debug-Pathfind"))
		{
			machine.ChangeState("EnemyWanderState", null);

		}

	}

	void EnterAttackState(Node2D node)
	{
		if (machine.CurrentState != "MoblinAttackState" && !isBusy)
		{
			if (node.GetType() == typeof(player))
			{
				machine.ChangeState("MoblinAttackState", null);
			}
		}
	}
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
		if(hitPoints>0)
			pathfinder.PathfinderPhysicsProcess(delta);
		else if (pathfinder.takingDamage)
		{
			pathfinder.HaltPathing();
		}
	}

    public void TakeDamage(int damage, HitBox2D box)
	{
		if(!isBusy)
		{
			
			machine.ChangeState("EnemyDamageState", new Dictionary<string, object>() { { "damage", damage },{ "hitBox", box } });
		}

    }
}
