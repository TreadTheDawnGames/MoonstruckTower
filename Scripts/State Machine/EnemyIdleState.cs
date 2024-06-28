using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class EnemyIdleState : EnemyState
{
    Timer chillinTimer;

    

    public override void SetUp(Dictionary<string, object> message)
    {
        base.SetUp(message);

        chillinTimer = GetNode<Timer>("Timer");
        chillinTimer.Timeout += () => StartWander();

    }

    

    public override void OnStart(Dictionary<string, object> message)
    {
        base.OnStart(message);

        

        animator.Play("Idle");
        statusAnimator.Play("None");

        chillinTimer.WaitTime = GD.RandRange(1, 5f);
        GD.Print(logic.Name + " is Idling for " + chillinTimer.WaitTime + " seconds");
        
        chillinTimer.Start();
    }

    void StartWander()
    {

        machine.ChangeState("EnemyWanderState", null);
    }

    public override void OnExit(string nextState)
    {
        base.OnExit(nextState);
        chillinTimer.Stop();
        
    }


}
