using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyBase : CharacterBody2D
{
    public AnimatedSprite2D animator;
    public AnimatedSprite2D statusAnimator;
    public EnemyStateMachine machine;
    public Node2D flippables;
    public Player link;
    protected private RayCast2D visionCast;
    public RayCast2D edgeDetectR;
    public RayCast2D edgeDetectL;
    public bool canSee = false;
    public CollisionShape2D hurtBox;
    public CollisionShape2D collisionBox;
    public HitBox2D hitBox;
    public Area2D passiveHitBox;

    public bool isBusy = false;
    public bool isAlerted = false;


    [Export]
    protected float sightRange = 160;
    [Export]
    protected int maxHP = 2;
    [Export]
    public float baseWalkSpeed = 75f;
    public float walkSpeed = 75f;
    [Export]
    public float jumpHeightPix = 16f;

    public int hitPoints;
    protected bool facingRight = false;

    public int walkDirection = 0;
    public bool takingDamage = false;
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    public Vector2 velocity;
    protected Vector2 startingPosition;
    public EnemySpawner spawner { protected get; set; }
    protected bool active = false;

    [Export] public SignalLock sLock;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        hitPoints = maxHP;
        collisionBox = GetNode<CollisionShape2D>(GetPath() + "/CollisionBox");
        animator = GetNode<AnimatedSprite2D>("Animator");
        statusAnimator = GetNode<AnimatedSprite2D>("Animator/StatusAnimator");
        machine = GetNode<EnemyStateMachine>("StateMachine");
        flippables = GetNode<Node2D>("Flippables");
        hurtBox = GetNode<CollisionShape2D>(flippables.GetPath() + "/HurtBox2D/CollisionShape2D");
        visionCast = GetNode<RayCast2D>("VisionCast");
        edgeDetectR = GetNode<RayCast2D>("EdgeDetectionR");
        edgeDetectL = GetNode<RayCast2D>("EdgeDetectionL");
        hitBox = GetNode<HitBox2D>("Flippables/HitBox2D");
        passiveHitBox = GetNode<Area2D>("Flippables/PassiveHitBox2D");

        startingPosition = GlobalPosition;
        animator.FlipH = GD.Randi() % 2 == 1 ? true : false;
        link = (Player)GetTree().GetFirstNodeInGroup("Player");



        animator.Play("Spawn");
    }
    protected void FlipDirection()
    {


        //int count = logic.edgeDetect.GetOverlappingBodies().Count;
        if (facingRight)
        {
            if (!edgeDetectR.IsColliding() || IsOnWall())
            {
                if (IsOnFloor())
                {
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
                    
                    walkDirection = -walkDirection;
                }
            }
        }

    }
    public void Destroy()
    {
        sLock?.Unlock();
        spawner.StartRespawnTimer();
        QueueFree();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (GlobalPosition.Y > startingPosition.Y + (16 * spawner.tilesToFallBeforeDeath))
        {
            hitPoints = 0;
            machine.ChangeState("EnemyDamageState", new Dictionary<string, object> { { "damage", 0 }, { "hitBox", null } });
        }
    }

    public void TakeDamage(int damage, HitBox2D box)
    {
        //if (!isBusy)
        {
            if (box.Owner is Projectile)
            {
                //get arrow owner script /detect if box was on projectile and if it was, delete it

                Projectile arrow = box.Owner.GetNode<Projectile>(box.Owner.GetPath());

                arrow.HitHurtBox();

            }
            machine.ChangeState("EnemyDamageState", new Dictionary<string, object>() { { "damage", damage }, { "hitBox", box } });
        }

    }
}
