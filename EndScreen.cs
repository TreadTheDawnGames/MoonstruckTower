using Godot;
using System;

public partial class EndScreen : Control
{

	Fader fader;
	Timer timer;
	Timer blackoutTimer;
	bool readyToExit = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		fader = GetNode<Fader>("Fader");
		timer = GetNode<Timer>("Timer");
		blackoutTimer = GetNode<Timer>("BlackoutTimer");

		fader.PlayIdle();
		blackoutTimer.Timeout += Exit; 
		fader.FadedIn += ()=> timer.Start();
		timer.Timeout += () => fader.FadeOut();
		fader.FadedOut += () => blackoutTimer.Start();
		PlayerPrefs.DeleteAll();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	void Exit()
	{
        if(readyToExit)
		GetTree().ChangeSceneToFile("res://Scenes/main_menu.tscn");
		else
		{
		fader.FadeIn();
			readyToExit = true;
		}
    }
}
