using Godot;
using System;

public partial class MainMenu : Control
{
	Fader fader;
	bool quitting;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		fader = GetNode<Fader>("Fader");
		fader.PlayIdle();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void StartPressed()
	{
		fader.FadeOut();
	}

	public void QuitPressed()
	{
		fader.FadeOut();
		quitting = true;
	}

	void FadeOut()
	{
		if(quitting)
			GetTree().Quit();
		else
	        GetTree().ChangeSceneToFile("res://Scenes/game.tscn");
    }
}
