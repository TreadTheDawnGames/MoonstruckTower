using Godot;
using System;
using System.Linq;

public partial class BossroomTorch : Node2D
{
	TorchLighting[] torches;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		torches = GetChildren().OfType<TorchLighting>().ToArray();
	}


	public void LightUp(bool isLit)
	{
		foreach (var torch in torches)
		{
			if (isLit)
			{
				torch.Animation = "default";
				torch.Play();
			}
			else
			{
				torch.Animation = "Unlit";
				torch.Play();
			}
		}
    }


}
