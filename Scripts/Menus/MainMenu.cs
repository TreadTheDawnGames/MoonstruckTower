
using Godot;
using System;

public partial class MainMenu : Control
{
	Fader fader;
	bool quitting;
    TextureButton startButton;
    TextureButton quitButton;

	SettingsWindow settingsWindow;
	bool resumingGame = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		fader = GetNode<Fader>("Fader");
		startButton = GetNode<TextureButton>("VBoxContainer/TextureButton");
		quitButton = GetNode<TextureButton>("VBoxContainer/TextureButton2");

		resumingGame = PlayerPrefs.GetBool("resumingGame", false);

		if (resumingGame)
		{
			startButton.TexturePressed = GD.Load<Texture2D>("res://Assets/UI/Buttons/ResumeClick.png");
            startButton.TextureHover = GD.Load<Texture2D>("res://Assets/UI/Buttons/ResumeHover.png");
            startButton.TextureNormal = GD.Load<Texture2D>("res://Assets/UI/Buttons/ResumeNormal.png");
        }

		var mode = PlayerPrefs.GetBool("Fullscreen", true) ? DisplayServer.WindowMode.Fullscreen:DisplayServer.WindowMode.Windowed;
        WindowSizer.Instance.ChangeWindowSize(mode);


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

		if (!resumingGame)
		{
	        PlayerPrefs.SetBool("resumingGame", true);
		}


        fader.MouseFilter = MouseFilterEnum.Stop;
		fader.FadeOut();
	}

	public void QuitPressed()
	{
		fader.FadeOut();
		quitting = true;
        fader.MouseFilter = MouseFilterEnum.Stop;
		GetTree().Root.GetNode<Ambience>("Ambient").fadeOut = true;
    }

    void QuitApp()
	{
		if(quitting)
			GetTree().Quit();
		else
	        GetTree().ChangeSceneToFile("res://Scenes/game.tscn");
    }
}
