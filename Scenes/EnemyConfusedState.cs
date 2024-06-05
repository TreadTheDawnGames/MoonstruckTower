using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyConfusedState : EnemyState
{
    Timer timer;
    int timesFlipped = 0;
    int timesToFlip;
    public override void SetUp(Dictionary<string, object> message)
    {
        base.SetUp(message);
        timer = GetNode<Timer>("Timer");
        timer.Timeout += () => FlipLook();
    }

    public override void OnStart(Dictionary<string, object> message)
    {
        base.OnStart(message);
        animator.Play("Idle");
        statusAnimator.Play("?");
        FlipLook();
        timesToFlip = (int)GD.RandRange(1, 2);
        timesFlipped = 0;
        GD.Print("Looking " + timesToFlip + " times");
        timer.WaitTime = GD.RandRange(0.5f, 1);
        timer.Start();
    }

    void FlipLook()
    {
        animator.FlipH = !animator.FlipH; 
        if (timesFlipped <= timesToFlip)
        {

            timer.WaitTime = GD.RandRange(0.5f, 1);
            timer.Start();
            GD.Print("Looked " + timesFlipped + " times");
            timesFlipped++;
            return;
        }
        machine.ChangeState("EnemyWanderState", null);
    }
    public override void OnExit(string nextState)
    {
        base.OnExit(nextState);
        timer.Stop();
        timesToFlip = 0;
        timesFlipped = 0;
        statusAnimator.Play("None");
    }
}
