using Godot;
using System;

public partial class TorchLighting : AnimatedSprite2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AnimationFinished += StartNewAnimation;
		Play();
	}

	void StartNewAnimation()
	{
		double rand = GD.RandRange(0.75f, 1f);

		Play(customSpeed: (float)rand);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
