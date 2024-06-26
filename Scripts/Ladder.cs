using Godot;
using System;
using System.Linq;

public partial class Ladder : Node2D
{
     Area2D climbableSurface;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		climbableSurface = GetNode<Area2D>("ClimbableSurface");
		climbableSurface.BodyEntered += (node) => SetClimbing( node, true);
		climbableSurface.BodyExited += (node) => SetClimbing( node, false);
		
	}

    

    void SetClimbing(Node node, bool isClimbing)
	{
		if (node is Player)
		{

			

            Player link = (Player) node;
			link.ladderCount += isClimbing ? 1 : -1;
            node.Call("SetClimb", isClimbing);
			

			GD.Print("Set Climbing to " + isClimbing);
		}
	}


	

    
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
