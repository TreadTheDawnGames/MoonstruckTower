using Godot;
using System;

public partial class BossRoom : Node2D
{
	Fader fader;
	Area2D endGameZone;
	bool topReached = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		fader = GetNodeOrNull<Fader>(GetParent().GetPath()+"/CanvasLayer/Fader");
		endGameZone = GetNode<Area2D>("GameEndZone");
		endGameZone.BodyEntered += Exit;
		fader.FadedOut += EndGame;
	}

	void EndGame()
	{
		if (topReached)
		{
			GetTree().ChangeSceneToFile("res://Scenes/end_screen.tscn");
		}
	}

	void Exit(Node2D node)
	{
		if (fader != null)
			fader.FadeOut();

		if(node is Player)
		{
			topReached = true;
			node.ProcessMode = ProcessModeEnum.Disabled;
		}
	}
}
