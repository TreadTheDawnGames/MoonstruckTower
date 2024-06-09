using Godot;
using System;

public partial class EnemyV2 : Node2D
{
	public EnemyPathfinder pathfinder;
	public AnimatedSprite2D animator;
	public AnimatedSprite2D statusAnimator;
	public EnemyStateMachine machine;
	public Node2D flippables;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		pathfinder = GetNode<EnemyPathfinder>(Owner.GetPath());
		animator = GetNode<AnimatedSprite2D>("Animator");
		statusAnimator = GetNode<AnimatedSprite2D>("StatusAnimator");
		machine = GetNode<EnemyStateMachine>("StateMachine");
		flippables = GetNode<Node2D>("Flippables");

		//machine.SetUp();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (pathfinder.movementDirection > 0)
		{
			animator.FlipH = true;
			flippables.Scale = new Vector2(-1, 1);
		}
        else if(pathfinder.movementDirection < 0) 
        {
			animator.FlipH = false;
            flippables.Scale = new Vector2(1, 1);
        }

		if (Input.IsActionJustPressed("Debug-Pathfind"))
		{
			pathfinder.CreateAndGoToPath(GetGlobalMousePosition());

		}

    }
}
