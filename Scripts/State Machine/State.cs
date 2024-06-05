using Godot;
using System.Collections.Generic;
using System;

public partial class State : Node
{
	private bool HasBeenInititalized = false;
	private bool OnUpdateHasFired = false;

	[Signal] public delegate void StateStartEventHandler();
	[Signal] public delegate void StateUpdatedEventHandler();
	[Signal] public delegate void StateExitedEventHandler();

	public virtual void OnStart(Dictionary<string, object> message)
	{
		EmitSignal(nameof(StateStart));
		HasBeenInititalized = true;
	}

	public virtual void OnUpdate()
	{
		if (!HasBeenInititalized)
			return;
		EmitSignal(nameof(StateUpdated));
		OnUpdateHasFired = true;
	}

	public virtual void UpdateState(float delta)
	{
		if (!OnUpdateHasFired)
			return;
	}

	public virtual void OnExit(string nextState)
	{
		if (!HasBeenInititalized)
			return;
		EmitSignal(nameof(StateExited));
		HasBeenInititalized = false;
		OnUpdateHasFired = false;
	}

	public virtual void SetUp(Dictionary<string, object> message)
	{

	}
}
