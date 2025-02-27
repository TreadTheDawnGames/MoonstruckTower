using Godot;
using System;
using System.Collections.Generic;
using System.Security;

public partial class EnemyWanderState : EnemyState
{

    
    Timer timer;

    public override void SetUp(Dictionary<string, object> message)
    {
        base.SetUp(message);


        timer = GetNode<Timer>("Timer");
    }

    // Called when the node enters the scene tree for the first time.
    public override void OnStart(Dictionary<string, object> message)
    {
        timer.Timeout += EndWander;
        base.OnStart(message);
        animator.Play("Walk");
        statusAnimator.Play("None");
        

            do
            {
                logic.walkDirection = Mathf.Sign(GD.RandRange(-1, 1));

            }
            while (logic.walkDirection == 0);
        

        timer.WaitTime = GD.RandRange(0.5f, 2f);
        timer.Start();
    }

    public  void StopWandering()
    {
        if (isCurrentState)
        {
            logic.walkDirection = 0;
            machine.ChangeState("EnemyIdleState", null);
        }
    }

    void EndWander()
    {
        machine.ChangeState("EnemyIdleState", null);
    }

    

    public override void OnExit(string nextState)
    {
        base.OnExit(nextState);
        timer.Stop();
        logic.walkDirection = 0;
        timer.Timeout -= EndWander;

    }

}
