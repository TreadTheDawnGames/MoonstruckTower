using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class StateMachine : Node
{	
	[Signal] public delegate void PreStartEventHandler();
	[Signal] public delegate void PostStartEventHandler();
	[Signal] public delegate void PreExitEventHandler();
	[Signal] public delegate void PostExitEventHandler();

	public List<State> States;

	public string CurrentState;
	public string LastState;
	protected State state = null;

    public override void _Ready()
    {
		States = GetNode<Node>("States").GetChildren().OfType<State>().ToList();
		
		
		
    }

	private void SetState(State _state, Dictionary<string, object> message)
	{ 
		if (_state == null)
			return;
		if(_state != null)
		{
			EmitSignal(nameof(PreExit));
			if(state!=null)
				state.OnExit(_state.GetType().ToString());
			EmitSignal(nameof(PostExit));
		}

        LastState = CurrentState;
		CurrentState = _state.GetType().ToString();

		state = _state;
		EmitSignal(nameof(PreStart));
		state.OnStart(message);
		EmitSignal(nameof(PostStart));
		state.OnUpdate();

	}

	public void ChangeState(string stateName, Dictionary<string, object> message)
	{
	
		foreach(State _state in States)
		{
			string compareTo = _state.GetType().ToString();
			if(stateName == compareTo)
			{
				SetState(_state, message);
				return;
			}
		}
		GD.PrintErr("No state of type " + $"{stateName}");
	}

    public override void _PhysicsProcess(double delta)
    {
		if (state != null)
		{
            state.UpdateState((float)delta);
		}
	}
}
