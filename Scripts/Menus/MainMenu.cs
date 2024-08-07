using Godot;
using System;

public partial class MainMenu : Control
{
	Fader fader;
	bool quitting;
    TextureButton startButton;
    TextureButton quitButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		fader = GetNode<Fader>("Fader");
		startButton = GetNode<TextureButton>("VBoxContainer/TextureButton");
		quitButton = GetNode<TextureButton>("VBoxContainer/TextureButton2");

		startButton.ButtonDown += StartPressed;
		quitButton.ButtonDown += QuitPressed;
        fader.MouseFilter = MouseFilterEnum.Ignore;
		fader.FadeIn();

    }



    public void StartPressed()
	{
		fader.FadeOut();
	}

	public void QuitPressed()
	{
		fader.FadeOut();
		quitting = true;
        fader.MouseFilter = MouseFilterEnum.Stop;

    }

    void QuitApp()
	{
		if(quitting)
			GetTree().Quit();
		else
	        GetTree().ChangeSceneToFile("res://Scenes/game.tscn");
    }
}
