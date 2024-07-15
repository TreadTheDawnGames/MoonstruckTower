using Godot;
using System;

public partial class DebugTools : Node2D
{

	Fader fader;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		fader = GetNodeOrNull<Fader>("CanvasLayer/Fader");
		fader?.FadeIn();
    }

	bool slowTimeToggle = false;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Debug-Reload"))
		{
			GetTree().ReloadCurrentScene();
		}
		
		if (Input.IsActionJustPressed("Debug-TimeSlowToggle"))
		{
			slowTimeToggle = !slowTimeToggle;
			Engine.TimeScale = slowTimeToggle ? 0.5f : 1f;
			GD.Print("Time slowed = " + slowTimeToggle);
		}
	}
}
