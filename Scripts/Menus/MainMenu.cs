
using Godot;
using System;

public partial class MainMenu : Control
{
	Fader fader;
	bool quitting;
    TextureButton startButton;
    TextureButton quitButton;

	SettingsWindow settingsWindow;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		fader = GetNode<Fader>("Fader");
		startButton = GetNode<TextureButton>("VBoxContainer/TextureButton");
		quitButton = GetNode<TextureButton>("VBoxContainer/TextureButton2");

		startButton.Pressed += StartPressed;
		quitButton.Pressed += QuitPressed;
        fader.MouseFilter = MouseFilterEnum.Ignore;
		fader.FadedOut += QuitApp;
		//fader.FadeIn();

		settingsWindow = GetNodeOrNull<SettingsWindow>("VBoxContainer/OpenSettingButton/SettingsWindow");

		if (GetTree().Paused)
		{
			GetTree().Paused = false;
            AudioServer.SetBusEffectEnabled(1, 0, false);

        }

        if (!GetTree().Root.HasNode("Ambient"))
		{
			PackedScene ambient = GD.Load<PackedScene>("res://Scenes/ambient.tscn");
			Node2D scene = ambient.Instantiate<Node2D>();
			GetTree().Root.CallDeferred("add_child", scene);
		}

    }
	



    public void StartPressed()
	{
		if(GetTree().Paused)
		{
			GetTree().Paused = false;
		}

		if (settingsWindow.windowOpen)
		{
			settingsWindow.Animate();
		}

        fader.MouseFilter = MouseFilterEnum.Stop;
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
