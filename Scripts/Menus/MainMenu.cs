
using Godot;
using System;
using System.Linq;

public partial class MainMenu : Control
{
	Fader fader;
	bool quitting;
    TextureButton startButton;
    TextureButton quitButton;
    TextureButton iButton;

	SettingsWindow settingsWindow;

	public static MainMenu instance;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		instance = this;

		fader = GetNode<Fader>("Fader");
		startButton = GetNode<TextureButton>("VBoxContainer/TextureButton");
		quitButton = GetNode<TextureButton>("VBoxContainer/TextureButton2");
		iButton = GetNode<TextureButton>("VBoxContainer/OpenSettingButton");


		SetPlayButtonTextures();

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
		PauseMenu.SetCursorVisible(true);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

		if(!settingsWindow.windowOpen) 
		if(@event is InputEventJoypadButton && !startButton.HasFocus() && !quitButton.HasFocus() && !iButton.HasFocus())
		{
			startButton.GrabFocus();
		}
    }

    public void SetPlayButtonTextures()
	{
        if (PlayerPrefs.GetBool("resumingGame", false))
        {
            startButton.TexturePressed = GD.Load<Texture2D>("res://Assets/UI/Buttons/ResumeClick.png");
            startButton.TextureHover = GD.Load<Texture2D>("res://Assets/UI/Buttons/ResumeHover.png");
            startButton.TextureNormal = GD.Load<Texture2D>("res://Assets/UI/Buttons/ResumeNormal.png");
            startButton.TextureFocused = GD.Load<Texture2D>("res://Assets/UI/Buttons/ResumeFocusedBlack.png");
        }
		else
        {
            startButton.TexturePressed = GD.Load<Texture2D>("res://Assets/UI/Buttons/StartButton-Clicked.png");
            startButton.TextureHover = GD.Load<Texture2D>("res://Assets/UI/Buttons/StartButton-Hover.png");
            startButton.TextureNormal = GD.Load<Texture2D>("res://Assets/UI/Buttons/StartButton-Normal.png");
            startButton.TextureFocused = GD.Load<Texture2D>("res://Assets/UI/Buttons/StartButtonFocused.png");
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

		PauseMenu.SetCursorVisible(false);

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
