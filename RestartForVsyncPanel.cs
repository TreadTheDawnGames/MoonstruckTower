using Godot;
using Godot.NativeInterop;
using System;

public partial class RestartForVsyncPanel : TexturePanel_YesNoDialog
{
    public override void _Ready()
    {
        base._Ready();
        if (GetTree().Root.HasNode("LevelScene"))
        {
            Texture = GD.Load<Texture2D>("res://Assets/UI/RestartForVSyncPanel_DataWillBeSaved.png");
        }
        else
        {
            Texture = GD.Load<Texture2D>("res://Assets/UI/RestartForVSyncPanel.png");
        }
    }

    protected override void YesPressed()
    {
        WindowSizer.Instance.SetVsync(false);
        
/*        SettingsWindow.Vsync = false;
        SettingsWindow.VsyncButton.TexturePressed = GD.Load<Texture2D>("res://Assets/UI/Buttons/VSyncDisabledClick.png");
        SettingsWindow.VsyncButton.TextureHover = GD.Load<Texture2D>("res://Assets/UI/Buttons/VSyncDisabledHover.png");
        SettingsWindow.VsyncButton.TextureNormal = GD.Load<Texture2D>("res://Assets/UI/Buttons/VSyncDisabledNormal.png");
*/
        OS.SetRestartOnExit(true);

        GetTree().Quit();
    }

    protected override void NoPressed()
    {
        base.NoPressed();
    }
}
