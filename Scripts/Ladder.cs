using Godot;
using System;
using System.Linq;

public partial class Ladder : RigidBody2D
{
	Area2D upCast;
	Area2D downCast;
     Area2D climbableSurface;
	CollisionShape2D shape;
	AnimatedSprite2D animator;

	CollisionShape2D topFloor;
	CollisionShape2D bottomFloor;

	AudioPlayer audioPlayer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		animator = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		climbableSurface = GetNode<Area2D>("ClimbableSurface");
		audioPlayer = GetNode<AudioPlayer>("AudioStreamPlayer2D");
		climbableSurface.BodyEntered += (node) => SetClimbing( node, true);
		climbableSurface.BodyExited += (node) => SetClimbing( node, false);




		shape = GetNode<CollisionShape2D>("CollisionShape2D");
		upCast = GetNode<Area2D>("UpCast");
		downCast = GetNode<Area2D>("DownCast");

		topFloor = GetNode<CollisionShape2D>("EndCaps/TopFloor");
		bottomFloor = GetNode<CollisionShape2D>("EndCaps/BottomFloor");
/*
		upCast.BodyEntered += (node) => ChangeFloorOn(bottomFloor, true);
		upCast.BodyExited += (node) => ChangeFloorOn(bottomFloor, false);
		downCast.BodyEntered += (node) => ChangeFloorOn(topFloor, true);
		downCast.BodyExited += (node) => ChangeFloorOn(topFloor, false);*/

        
		animator.AnimationFinished += AnimationFinished;

		
	}

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if(upCast.HasOverlappingBodies())
		{
            bottomFloor.Disabled = false;
        }
        else
		{
			bottomFloor.Disabled = true;
		}

        if (downCast.HasOverlappingBodies())
        {
            topFloor.Disabled = false;
        }
        else
        {
            topFloor.Disabled = true;
        }


    }

    void ChangeFloorOn(CollisionShape2D shape, bool active)
	{
		shape.SetDeferred("disabled", !active);

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
			Player playerChar = (Player)node;
			playerChar.ladderCount += isClimbing ? 1 : -1;
			node.Call("SetClimb", isClimbing);
		}
	}

	public void Despawn(bool animate)
	{
		if (animate)
		{
			animator.Play("Despawn");
			audioPlayer.Play();
		}
		else
		{
            shape.SetDeferred("disabled", true);
            QueueFree();
        }
    }
}
