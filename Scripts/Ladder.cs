using Godot;
using System;
using System.Linq;

public partial class Ladder : Node2D
{
     Area2D climbableSurface;
	CollisionShape2D shape;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		climbableSurface = GetNode<Area2D>("ClimbableSurface");
		climbableSurface.BodyEntered += (node) => SetClimbing( node, true);
		climbableSurface.BodyExited += (node) => SetClimbing( node, false);
		shape = GetNode<CollisionShape2D>("CollisionShape2D");
		
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

	public void Despawn()
	{
		shape.SetDeferred("disabled", true);
		QueueFree();
	}
}
