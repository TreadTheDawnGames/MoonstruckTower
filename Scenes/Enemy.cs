using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class Enemy : CharacterBody2D
{

	[Export] public int hitPoints = 3;
	[Export] public float speed = 300f;
    [Export] public float jumpPower = -275;
	public AnimatedSprite2D animator;
	public AnimatedSprite2D statusAnimator;
	public CollisionShape2D hurtBox;
	public CollisionShape2D hitBox;
	public CollisionShape2D collisionBox;
	EnemyStateMachine machine;
	public RayCast2D wallDetection;
	public RayCast2D jumpDetection;
	public Node2D flippables;
	public Area2D floorCheck;
	public bool hasTarget = false;

    public Area2D fov;

    bool takingDamage = false;
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{


		if (GetNodeOrNull("HitBox2D/HitBoxShape") != null)
			hitBox = (CollisionShape2D)GetNode("Flippables/HitBox2D/HitBoxShape");

		
        flippables = GetNode<Node2D>("Flippables");
        wallDetection = GetNode<RayCast2D>("Flippables/WallDetection");
        jumpDetection = GetNode<RayCast2D>("Flippables/JumpDetection");
        hurtBox = (CollisionShape2D)GetNode("Flippables/HurtBox2D/HurtBoxShape");
		animator = GetNode<AnimatedSprite2D>("Animator");
		statusAnimator = animator.GetNode<AnimatedSprite2D>("StatusAnimator");
		machine = GetNodeOrNull<EnemyStateMachine>("StateMachine");
		collisionBox = (CollisionShape2D)GetNode("CollisionBox");
		fov = GetNode<Area2D>("Flippables/FOV");
		floorCheck = GetNode<Area2D>("FloorCheck");

		machine?.Setup();
		statusAnimator.Play("None");

	}
    

	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public void DoMove(double delta, Vector2 movement)
	{
		if (animator.FlipH)
		{
			flippables.Scale = new Vector2(-1, 1);
		}
		else
		{
			flippables.Scale = new Vector2(1, 1);
		}
		
		if (!IsOnFloor())
		{
			movement.Y += gravity * (float)delta;

		}
		Velocity = movement;
		if (!takingDamage)
		{
			MoveAndSlide();
		}
	}


    public void TakeDamage(int damage)
	{
		machine.ChangeState("EnemyDamageState", new Dictionary<string, object>() { { "damage", damage } }) ;

	}

	public void Destroy()
	{
		QueueFree();
	}
}
