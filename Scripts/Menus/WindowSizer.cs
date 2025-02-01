using Godot;
using System;

public partial class WindowSizer : Node
{
    private static WindowSizer instance = null;
    private WindowSizer()
    {
    }

    public static WindowSizer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new WindowSizer();
            }
            return instance;
        }
    }

    public override void _Ready()
    {
        base._Ready();
        instance = this;
        if (OS.IsRestartOnExitSet()) OS.SetRestartOnExit(false);
    }

    public void SetVsync(bool enabled)
    {
        DisplayServer.VSyncMode mode = enabled ? DisplayServer.VSyncMode.Enabled : DisplayServer.VSyncMode.Disabled;
        DisplayServer.WindowSetVsyncMode(mode);

        PlayerPrefs.SetBool("VsyncEnabled", enabled);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Input.IsActionJustPressed("ToggleFullscreen"))
        {
            var mode = DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Windowed ? DisplayServer.WindowMode.Fullscreen : DisplayServer.WindowMode.Windowed;

            Instance.ChangeWindowSize(mode);

            PlayerPrefs.SetBool("Fullscreen", mode == DisplayServer.WindowMode.Fullscreen);
        }
    }

    public void ChangeWindowSize(DisplayServer.WindowMode mode)
    {
        DisplayServer.WindowSetMode(mode);
        if (mode == DisplayServer.WindowMode.Windowed)
        {
            DisplayServer.WindowSetSize(new Vector2I(1152, 648));

        }
    }
}
