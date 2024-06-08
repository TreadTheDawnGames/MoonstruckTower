using Godot;
using System;

public partial class GraphPoint : Sprite2D
{
	public Timer timer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timer = GetNode<Timer>("Timer");
		timer.Timeout += () => QueueFree();
	}

	
}
