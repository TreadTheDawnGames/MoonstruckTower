using Godot;
using System;

public partial class DebugTools : Node2D
{
	bool paused = false;

	PauseMenu pauseMenu;

	public Fader fader;

	Player player;

	[Signal]
	public delegate void SpawnFinishedEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		fader = GetNodeOrNull<Fader>("CanvasLayer/Fader");
		pauseMenu = GetNodeOrNull<PauseMenu>("CanvasLayer/PauseMenu");

		player = GetNode<Player>("Player");

		//fader?.FadeIn();
    }

    public override void _Notification(int what)
    {
        base._Notification(what);
		if (what == NotificationWMCloseRequest)
		{
			pauseMenu.SaveGame();
		}
    }
    public void FinishSpawning()
	{
		EmitSignal(SignalName.SpawnFinished);
	}
	

	bool slowTimeToggle = false;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        /*if (Input.IsActionJustPressed("Debug-Reload"))
		{
			GetTree().ReloadCurrentScene();
		}
		
		if (Input.IsActionJustPressed("Debug-TimeSlowToggle"))
		{
			slowTimeToggle = !slowTimeToggle;
			Engine.TimeScale = slowTimeToggle ? 0.5f : 1f;
			//GD.Print("Time slowed = " + slowTimeToggle);
		}

		if (Input.IsActionJustPressed("Debug-ResetPrefs"))
		{
			//GD.Print("Deleted ALL PlayerPrefs");
			PlayerPrefs.DeleteAll();
		}*/

        

    }
}
