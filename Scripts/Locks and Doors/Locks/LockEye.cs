using Godot;
using System;

public partial class LockEye : Lock
{
	

    protected override void UnlockMe()
	{
		if (!unlocked)
		{
			base.UnlockMe();

			sprite.Play("Close");
			door.Open();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
