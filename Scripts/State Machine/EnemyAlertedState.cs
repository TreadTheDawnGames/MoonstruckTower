using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyAlertedState : EnemyState
{
    Timer timer;
    bool alertTimeout = false;
    public override void SetUp(Dictionary<string, object> message)
    {
        base.SetUp(message);
        timer = GetNode<Timer>("Timer");
    }

    public override void OnStart(Dictionary<string, object> message)
    {
        base.OnStart(message);
        statusAnimator.Play("!");
        logic.isAlerted = true;
        timer.Timeout += SetAlertTimeout;
        timer.Start();
    }

    void SetAlertTimeout()
    {
        logic.pathfinder.HaltPathing();
        alertTimeout = true;
			machine.ChangeState("EnemyChaseState",null);
    }

    public override void UpdateState(float delta)
    {
        base.UpdateState(delta);
        bool onFloor = logic.pathfinder.IsOnFloor();
        //if (/*onFloor &&*/ /*alertTimeout &&*/!logic.isBusy)
		{
		}
    }

    public override void OnExit(string nextState)
    {
        base.OnExit(nextState);
        timer.Timeout -= SetAlertTimeout;
        alertTimeout = false;
    }
}
