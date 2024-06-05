using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyWanderState : EnemyState
{
    int wanderDirection;
    Timer timer;

    public override void SetUp(Dictionary<string, object> message)
    {
        base.SetUp(message);
    
    
        timer = GetNode<Timer>("Timer");
        timer.Timeout += () => EndWander();
    }

    // Called when the node enters the scene tree for the first time.
    public override void OnStart(Dictionary<string, object> message)
    {
        base.OnStart(message);
        animator.Play("Walk");
        do
        {
            wanderDirection = Mathf.Sign(GD.RandRange(-1, 1));

        }
        while (wanderDirection == 0);

        timer.WaitTime = GD.RandRange(0.5f, 2f);
        GD.Print(body.Name + " is Wandering for " + timer.WaitTime + " seconds");
        timer.Start();
    }

    void EndWander()
    {
        GD.Print("End wander");
        machine.ChangeState("EnemyIdleState", null);
    }

    public override void UpdateState(float delta)
    {
        moveDirection = wanderDirection;
        base.UpdateState(delta);


    }

    public override void OnExit(string nextState)
    {
        base.OnExit(nextState);
        timer.Stop();
    }

}
