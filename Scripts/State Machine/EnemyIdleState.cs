using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class EnemyIdleState : EnemyState
{
    Timer chillinTimer;
    [Export] float idleMin = 1, idleMax = 5;
    

    public override void SetUp(Dictionary<string, object> message)
    {
        base.SetUp(message);

        chillinTimer = GetNode<Timer>("Timer");

    }

    

    public override void OnStart(Dictionary<string, object> message)
    {
        base.OnStart(message);
        chillinTimer.Timeout += StartWander;



        animator.Play("Idle");
        statusAnimator.Play("None");

        chillinTimer.WaitTime = GD.RandRange(idleMin, idleMax);
        
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
        chillinTimer.Timeout -= StartWander;
        

    }


}
