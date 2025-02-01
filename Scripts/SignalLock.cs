using Godot;
using System;

	[Icon("res://Assets/Locks and Doors/Icons/SignalLockIcon.png")]
public partial class SignalLock : Lock
{
    // Called when the node enters the scene tree for the first time.
    public override void SetUp()
	{
		base.SetUp();
	}

	public void Unlock()
	{
		UnlockMe(null);
	}

	public void Lock()
	{
		LockMe();
	}

}
