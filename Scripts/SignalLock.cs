using Godot;
using System;

public partial class SignalLock : Lock
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void Unlock()
	{
		UnlockMe(null);
	}

	public void Lock()
	{
		LockMe();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
