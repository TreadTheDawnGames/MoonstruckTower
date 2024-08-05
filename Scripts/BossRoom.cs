using Godot;
using System;

public partial class BossRoom : Node2D
{
	Fader fader;
	Area2D area;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		fader = GetNodeOrNull<Fader>(GetParent().GetPath()+"/CanvasLayer/Fader");
		area = GetNode<Area2D>("GameEndZone");
		area.BodyEntered += Exit;
	}

	void Exit(Node2D node)
	{
		if (fader != null)
			fader.FadeOut();

		if(node is Player)
		{
			Player player = (Player)node;
			player.Hide();
			player.Speed = 0;
		}
	}
}
