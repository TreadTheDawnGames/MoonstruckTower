using Godot;
using System;
using System.Linq;

public partial class Ladder : RigidBody2D
{

     Area2D climbableSurface;
	CollisionShape2D shape;
	AnimatedSprite2D animator;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		animator = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		climbableSurface = GetNode<Area2D>("ClimbableSurface");
		climbableSurface.BodyEntered += (node) => SetClimbing( node, true);
		climbableSurface.BodyExited += (node) => SetClimbing( node, false);
		shape = GetNode<CollisionShape2D>("CollisionShape2D");
		animator.AnimationFinished += AnimationFinished;
	}

	void AnimationFinished()
	{
		if(animator.Animation== "Despawn")
		{
            shape.SetDeferred("disabled", true);
            QueueFree();
        }
	}


	void SetClimbing(Node node, bool isClimbing)
	{
		if (node is Player)
		{



			Player link = (Player)node;
			link.ladderCount += isClimbing ? 1 : -1;
			node.Call("SetClimb", isClimbing);


			GD.Print("Set Climbing to " + isClimbing);
		}
	}

	public void Despawn(bool animate)
	{

		if (animate)
		{
			animator.Play("Despawn");
		}
		else
		{
            shape.SetDeferred("disabled", true);
            QueueFree();
        }
    }
}
