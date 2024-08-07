using Godot;
using System;

public partial class Chest : AnimatedSprite2D
{

	Area2D area;
	public bool opened { get; private set; } = false;
    public enum Treasure { Bow, Ladder };
	Player link;
	PackedScene tool;

	[Export] public Treasure treasure { get; private set; }


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		area = GetNode<Area2D>("Area2D");
		area.AreaEntered += (node)=> Open(node);
		AnimationFinished += End;

		//opened = PlayerPrefs.GetBool("Opened"+Name, false);
		opened = PlayerPrefs.GetBool(Name + "Opened", false);
        Play( opened ? "Opened" : "Closed");

	}

	void End()
	{
		if (!opened)
		{
            //PlayerPrefs.SetBool("Opened"+Name, true);

            if (Animation == "AwardBow")
			{
				tool = GD.Load<PackedScene>("res://Scenes/Tools/Bow/bow.tscn");

			}
			if (Animation == "AwardLadder")
			{
				tool = GD.Load<PackedScene>("res://Scenes/Tools/Ladder/ladder_spawner.tscn");
				PlayerPrefs.SetBool("Ladder", true);
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
		if (!opened)
		{

			if (node.Owner is Player)
			{
				link = (Player)node.Owner;
				area.QueueFree();

				if (treasure == Treasure.Bow)
				{
					Play("AwardBow");
				}
				if (treasure == Treasure.Ladder)
				{
					Play("AwardLadder");
				}
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
