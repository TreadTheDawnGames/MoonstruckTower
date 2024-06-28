using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyV3 : CharacterBody2D
{
    public AnimatedSprite2D animator;
    public AnimatedSprite2D statusAnimator;
    public EnemyStateMachine machine;
    public Node2D flippables;
    public Player link;
    private RayCast2D visionCast;
    public RayCast2D edgeDetectR;
    public RayCast2D edgeDetectL;
    public bool canSee = false;
    public CollisionShape2D hurtBox;
    public CollisionShape2D collisionBox;
    public HitBox2D hitBox;
    public Area2D attackRangeArea;

    public bool isBusy = false;
    public bool isAlerted = false;


    [Export]
    float sightRange = 160;
    [Export]
    int maxHP = 2;
    [Export]
    public float baseWalkSpeed = 75f;
    public float walkSpeed = 75f;
    [Export]
    public float jumpHeightPix = 48f;

    public int hitPoints;
    bool facingRight = false;

    public int walkDirection = 0;
    public bool takingDamage = false;
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    public Vector2 velocity;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        hitPoints = maxHP;
        collisionBox = GetNode<CollisionShape2D>(GetPath() + "/CollisionBox");
        animator = GetNode<AnimatedSprite2D>("Animator");
        statusAnimator = GetNode<AnimatedSprite2D>("Animator/StatusAnimator");
        machine = GetNode<EnemyStateMachine>("StateMachine");
        flippables = GetNode<Node2D>("Flippables");
        hurtBox = GetNode<CollisionShape2D>(flippables.GetPath() + "/HurtBox2D/HurtShape");
        visionCast = GetNode<RayCast2D>("VisionCast");
        edgeDetectR = GetNode<RayCast2D>("EdgeDetectionR");
        edgeDetectL = GetNode<RayCast2D>("EdgeDetectionL");
        hitBox = GetNode<HitBox2D>("Flippables/HitBox2D");
        attackRangeArea = GetNode<Area2D>("Flippables/InRange");



        link = (Player)GetTree().GetFirstNodeInGroup("Player");

        machine.SetUp();
        attackRangeArea.BodyEntered += (node) => EnterAttackState(node);
    }



    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        //velocity = Velocity;
        if (!IsOnFloor())
        {
            velocity.Y += gravity * (float)delta;

        }
        

        var relitiveLinkPos = link.GlobalPosition - GlobalPosition;

        if (GlobalPosition.DistanceTo(link.GlobalPosition) < sightRange)
            visionCast.TargetPosition = relitiveLinkPos;
        else
            visionCast.TargetPosition = Vector2.Zero;

        //facing right
        if (Velocity.X > 0)
        {
            animator.FlipH = true;
            flippables.Scale = new Vector2(-1, 1);
            hitBox.damage = 1;
        }
        //facing left
        else if (Velocity.X < 0)
        {
            animator.FlipH = false;
            flippables.Scale = new Vector2(1, 1);
            hitBox.damage = -1;
        }
        facingRight = animator.FlipH;

        if (visionCast.GetColliderRid() == link.GetRid())
        {
            if (((Mathf.Sign(relitiveLinkPos.X) >= 0 && animator.FlipH) || (Mathf.Sign(relitiveLinkPos.X) <= 0 && !animator.FlipH)))
            {

                canSee = true;
            }
            else
            {
                canSee = false;
            }


            if (machine.CurrentState != "EnemyAlertState" && !isAlerted && canSee && !isBusy)
            {
                machine.ChangeState("EnemyAlertState", null);
            }

        }
        else
        {
            canSee = false;
        }

        if (walkDirection != 0)
        {
            FlipDirection();
            velocity.X = walkSpeed * walkDirection;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, walkSpeed);
        }

        Velocity = velocity;
        {
            MoveAndSlide();
        }

    }

    void EnterAttackState(Node2D node)
    {
        if (machine.CurrentState != "MoblinAttackState" && !isBusy)
        {
            if (node.GetType() == typeof(Player))
            {
                machine.ChangeState("MoblinAttackState", null);
            }
        }
    }
    void FlipDirection()
    {


        //int count = logic.edgeDetect.GetOverlappingBodies().Count;
        if (facingRight)
        {

            if (!edgeDetectR.IsColliding() || IsOnWall())
            {
                if (IsOnFloor())
                {
                    if (IsOnWall())
                    {
                        GD.Print("Flipping because on wall");
                    }
                    if (!edgeDetectR.IsColliding())
                    {
                        GD.Print("Flipping because on ledge");
                    }
                    GD.Print();
                    walkDirection = -walkDirection;
                }
            }
        }
        else
        {
            if (!edgeDetectL.IsColliding() || IsOnWall())
            {
                if (IsOnFloor())
                {
                    if (IsOnWall())
                    {
                        GD.Print("Flipping because on wall");
                    }
                    if (!edgeDetectR.IsColliding())
                    {
                        GD.Print("Flipping because on ledge");
                    }
                    GD.Print();
                    walkDirection = -walkDirection;
                }
            }
        }

    }
    public void Destroy()
    {
        QueueFree();
    }

    public void TakeDamage(int damage, HitBox2D box)
    {
        if (!isBusy)
        {

            machine.ChangeState("EnemyDamageState", new Dictionary<string, object>() { { "damage", damage }, { "hitBox", box } });
        }

    }
}

