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

		float masterVol = PlayerPrefs.GetFloat("MasterVolume", 3.5f);
		float musicVol = PlayerPrefs.GetFloat("MusicVolume", 3.5f);
		bool fullscreen = PlayerPrefs.GetBool("Fullscreen", true);

        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetBool("resumingGame", false);
		PlayerPrefs.SetFloat("MasterVolume", masterVol);
		PlayerPrefs.SetFloat("MusicVolume", musicVol);
        PlayerPrefs.SetBool("Fullscreen", fullscreen);

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
