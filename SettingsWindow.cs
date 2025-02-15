using Godot;
using System;
using System.Linq;

public partial class SettingsWindow : Control
{
    public bool windowOpen = false;
    AnimationPlayer animator;

    HSlider master, music;
    TextureButton xButton;
    public static TextureButton VsyncButton;
    public static TextureButton ResetButton;
    private PackedScene vsyncPanel = GD.Load<PackedScene>("res://Scenes/restart_panel.tscn");
    private PackedScene ResetPlayerPrefsPanel = GD.Load<PackedScene>("res://Scenes/reset_player_prefs_dialog.tscn");
    private Panel inputAbsorber;

    public static bool Vsync = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        animator = GetNode<AnimationPlayer>("AnimationPlayer");
        master = GetNode<HSlider>("TextureRect/Master");
        music = GetNode<HSlider>("TextureRect/Music");
        xButton = GetNode<TextureButton>("TextureRect/Close");
        VsyncButton = GetNode<TextureButton>("TextureRect/Vsync");
        ResetButton = GetNode<TextureButton>("TextureRect/ResetButton");
        inputAbsorber = GetNode<Panel>("TextureRect/InputAbsorber");
        inputAbsorber.Hide();

        xButton.FocusMode = FocusModeEnum.None;
        VsyncButton.FocusMode = FocusModeEnum.None;
        ResetButton.FocusMode = FocusModeEnum.None;
        master.FocusMode = FocusModeEnum.None;
        music.FocusMode = FocusModeEnum.None;

        master.Value = Mathf.DbToLinear(PlayerPrefs.GetValue("MasterVolume", master.MaxValue));
        music.Value = Mathf.DbToLinear(PlayerPrefs.GetValue("MusicVolume", music.MaxValue / 2));

        Vsync = PlayerPrefs.GetBool("VsyncEnabled", false);
        WindowSizer.Instance.SetVsync(Vsync);
        if (!Vsync)
        {
            VsyncButton.TexturePressed = GD.Load<Texture2D>("res://Assets/UI/Buttons/VSyncDisabledClick.png");
            VsyncButton.TextureHover = GD.Load<Texture2D>("res://Assets/UI/Buttons/VSyncDisabledHover.png");
            VsyncButton.TextureNormal = GD.Load<Texture2D>("res://Assets/UI/Buttons/VSyncDisabledNormal.png");
            VsyncButton.TextureFocused = GD.Load<Texture2D>("res://Assets/UI/Buttons/VSyncDisabledFocus.png");
        }
        else
        {
            VsyncButton.TexturePressed = GD.Load<Texture2D>("res://Assets/UI/Buttons/VSyncEnabledClick.png");
            VsyncButton.TextureHover = GD.Load<Texture2D>("res://Assets/UI/Buttons/VSyncEnabledHover.png");
            VsyncButton.TextureNormal = GD.Load<Texture2D>("res://Assets/UI/Buttons/VSyncEnabledNormal.png");
            VsyncButton.TextureFocused = GD.Load<Texture2D>("res://Assets/UI/Buttons/VSyncEnabledFocus.png");
        }


        master.ValueChanged += UpdateMasterVolume;
        master.DragEnded += SaveAudioSettings;

        music.ValueChanged += UpdateMusicVolume;
        music.DragEnded += SaveAudioSettings;

        xButton.Pressed += Animate;

        VsyncButton.Pressed += DoVsyncButton;

        ResetButton.Pressed += DoResetButton;

        if (GetTree().Root.HasNode("LevelScene"))
        {
            ResetButton.Disabled = true;
        }

    }


    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (windowOpen)
        {
            if (@event is InputEventJoypadButton && !master.HasFocus() && !music.HasFocus() && !xButton.HasFocus() && !VsyncButton.HasFocus() && !ResetButton.HasFocus() && !GetTree().HasGroup("YNDialog"))
            {
                xButton.GrabFocus();
                GetViewport().SetInputAsHandled();

            }

            if (@event.IsActionPressed("ui_back"))
            {
                var focusedControl = GetTree().Root.GetViewport().GuiGetFocusOwner();
                if (focusedControl.Equals(xButton))
                    Animate();
                else if (!TexturePanel_YesNoDialog.Shown)
                    xButton.GrabFocus();

                GetViewport().SetInputAsHandled();

            }

        }
    }

    void DoResetButton()
    {
        var thePanel = ResetPlayerPrefsPanel.Instantiate<TexturePanel_YesNoDialog>();
        thePanel.OpenDialogButton = ResetButton;
        AddChild(thePanel);
        thePanel.Show();
    }

    public void DoVsyncButton()
    {
        if (!Vsync)
        {
            Vsync = true;
            WindowSizer.Instance.SetVsync(true);
            VsyncButton.TexturePressed = GD.Load<Texture2D>("res://Assets/UI/Buttons/VSyncEnabledClick.png");
            VsyncButton.TextureHover = GD.Load<Texture2D>("res://Assets/UI/Buttons/VSyncEnabledHover.png");
            VsyncButton.TextureNormal = GD.Load<Texture2D>("res://Assets/UI/Buttons/VSyncEnabledNormal.png");
            VsyncButton.TextureFocused = GD.Load<Texture2D>("res://Assets/UI/Buttons/VSyncEnabledFocus.png");

        }
        else
        {

            var thePanel = vsyncPanel.Instantiate<TexturePanel_YesNoDialog>();
            thePanel.OpenDialogButton = VsyncButton;
            AddChild(thePanel);
            thePanel.Show();
        }

    }
    public void UpdateMasterVolume(double value)
    {
        var wantedVolume = Mathf.LinearToDb(value);
        AudioServer.SetBusVolumeDb(0, (float)wantedVolume);
    }
    public void UpdateMusicVolume(double value)
    {
        var wantedVolume = Mathf.LinearToDb(value);
        AudioServer.SetBusVolumeDb(1, (float)wantedVolume);
    }

    void SaveAudioSettings(bool isNewValue)
    {
        if (isNewValue)
        {
            PlayerPrefs.SetFloat("MasterVolume", AudioServer.GetBusVolumeDb(0));
            PlayerPrefs.SetFloat("MusicVolume", AudioServer.GetBusVolumeDb(1));
        }
    }

    public void Animate()
    {

        if (animator.IsPlaying())
        {
            animator.SpeedScale *= -1;
        }
        else
        {
            animator.SpeedScale = 1;

            if (windowOpen)
            {
                animator.PlayBackwards("In");
            }
            else
            {
                animator.Play("In");
            }
        }
        windowOpen = !windowOpen;

        DoFocusModeChange(windowOpen);
    }

    private void DoFocusModeChange(bool windowOpen)
    {
        if (windowOpen)
        {
            xButton.FocusMode = FocusModeEnum.All;
            VsyncButton.FocusMode = FocusModeEnum.All;
            if (!ResetButton.Disabled)
                ResetButton.FocusMode = FocusModeEnum.All;
            master.FocusMode = FocusModeEnum.All;
            music.FocusMode = FocusModeEnum.All;
            xButton.GrabFocus();
            inputAbsorber.Show();
        }
        else
        {
            xButton.FocusMode = FocusModeEnum.None;
            VsyncButton.FocusMode = FocusModeEnum.None;
            if (!ResetButton.Disabled)
                ResetButton.FocusMode = FocusModeEnum.None;
            master.FocusMode = FocusModeEnum.None;
            music.FocusMode = FocusModeEnum.None;

            GetParent<Control>().GrabFocus();
            inputAbsorber.Hide();
        }
    }
}
