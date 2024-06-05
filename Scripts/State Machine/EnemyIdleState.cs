using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class EnemyIdleState : EnemyState
{
    Timer timer;

    

    public override void SetUp(Dictionary<string, object> message)
    {
        base.SetUp(message);
        timer = GetNode<Timer>("Timer");
        timer.Timeout += () => StartWander();

    }

    public override void OnStart(Dictionary<string, object> message)
    {
        base.OnStart(message);
        animator.Play("Idle");
        timer.WaitTime = GD.RandRange(0.5f, 2f);
        GD.Print(body.Name + " is Idling for " + timer.WaitTime + " seconds");
        
        timer.Start();
    }

    void StartWander()
    {

        machine.ChangeState("EnemyWanderState", null);
    }

    public override void OnExit(string nextState)
    {
        base.OnExit(nextState);
        timer.Stop();
        
    }


}
