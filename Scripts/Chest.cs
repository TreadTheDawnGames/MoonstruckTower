using Godot;
using System;

public partial class Chest : AnimatedSprite2D
{

	Area2D area;
	bool opened = false;
	enum Treasure { Bow, Ladder };
	Player link;
	PackedScene tool;

	[Export] Treasure treasure;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		area = GetNode<Area2D>("Area2D");
		area.AreaEntered += (node)=> Open(node);
		AnimationFinished += End;
		Play("Closed");

	}

	void End()
	{
		if (!opened)
		{
			if (Animation == "AwardBow")
			{
				tool = GD.Load<PackedScene>("res://Scenes/Tools/Bow/bow.tscn");
			}
			if (Animation == "AwardLadder")
			{
				tool = GD.Load<PackedScene>("res://Scenes/Tools/Ladder/ladder_spawner.tscn");

			}
			if (tool != null)
			{

				opened = true;
				var addedTool = tool.Instantiate();
				link.toolBag.AddChild(addedTool);
				link.UpdateToolbag();
				Play("Opened");
			}
		}

	}
	void Open(Node2D node)
	{
		if (node.Owner is Player)
        {
		GD.Print(node.Owner);
			link = (Player)node.Owner;
			area.QueueFree();

			if(treasure == Treasure.Bow)
			{
				Play("AwardBow");
			}if(treasure == Treasure.Ladder)
			{
				Play("AwardLadder");
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
