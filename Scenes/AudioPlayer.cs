using Godot;
using System;

public partial class AudioPlayer : AudioStreamPlayer2D
{

    public override void _Ready()
    {
        base._Ready();
        MaxDistance = 200f;
    }
    public void PlaySound(AudioStream sound)
	{
        Stream = sound;
        Play();
    }
}
