using Godot;
using System;

public partial class SettingsButton : ButtonSounds
{
	[Export]
	SettingsWindow window;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		window = GetNode<SettingsWindow>("SettingsWindow");
		Pressed += window.Animate;
	}

}
