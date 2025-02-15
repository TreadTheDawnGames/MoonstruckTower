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
        base.YesPressed();
        WindowSizer.Instance.SetVsync(false);
        OS.SetRestartOnExit(true);
        GetTree().Quit();
    }

    protected override void NoPressed()
    {
        base.NoPressed();
    }
}
