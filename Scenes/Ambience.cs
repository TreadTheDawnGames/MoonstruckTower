using Godot;
using System;
using System.Diagnostics.Metrics;

public partial class Ambience : Node2D
{
	AudioStreamPlayer music, sounds;
	bool changed = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        AudioServer.SetBusVolumeDb(0, PlayerPrefs.GetValue("MasterVolume", 0f));
		AudioServer.SetBusVolumeDb(1, PlayerPrefs.GetValue("MusicVolume", 0f));

        music = GetNode<AudioStreamPlayer>("Music");
		sounds = GetNode<AudioStreamPlayer>("Sounds");

		
	}

	
}
