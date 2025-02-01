using Godot;
using System;
using System.Diagnostics.Metrics;

public partial class Ambience : Node2D
{
	AudioStreamPlayer musicPlayer, sounds;
	bool changed = false;
    public bool fadeOut;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        AudioServer.SetBusVolumeDb(0, PlayerPrefs.GetValue("MasterVolume", 0f));
		AudioServer.SetBusVolumeDb(1, PlayerPrefs.GetValue("MusicVolume", 0f));

        musicPlayer = GetNode<AudioStreamPlayer>("Music");
		sounds = GetNode<AudioStreamPlayer>("Sounds");

       
        
            // musicPlayer.Reparent(GetTree().Root, false);
            //musicPlayer.CallDeferred("reparent", GetTree().Root, false);
            //musicPlayer.Stream = GD.Load<AudioStream>("res://Assets/Sounds/CalmPiggiesLoop.wav");
            musicPlayer.VolumeDb = -40;
            musicPlayer.Play();
        
    }
    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!fadeOut && musicPlayer.VolumeDb < -5)
            musicPlayer.VolumeDb += 25 * (float)delta;

        if (fadeOut && musicPlayer.VolumeDb > -80)
            musicPlayer.VolumeDb -= 25 * (float)delta;
    }


}
