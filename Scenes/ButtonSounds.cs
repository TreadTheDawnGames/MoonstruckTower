using Godot;
using System;

public partial class ButtonSounds : TextureButton
{
	[Export]
	AudioStream hovered, clicked;

	AudioStreamPlayer audioPlayer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		hovered = GD.Load<AudioStream>("res://Assets/Audio/SFX/Menus/ButtonHover.wav");
		clicked = GD.Load<AudioStream>("res://Assets/Audio/SFX/Menus/ButtonClick.wav");
		audioPlayer = Owner.GetNode<AudioStreamPlayer>("ButtonAudio");
		MouseEntered += Hovered;
		Pressed += Clicked;
	}

	void Hovered()
	{
		audioPlayer.Stream = hovered;
		audioPlayer.Play();
	}

	void Clicked()
	{
		audioPlayer.Stream = clicked;
		audioPlayer.Play();
	}
}
