using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyChaseState : EnemyState
{
    Player playerChar;
    Area2D rangeArea;
    Timer repathTimer;
    private Vector2 lastLocation;
    public Vector2 LastLocation { get { return lastLocation; } set { if (lastLocation != value) locationUpdated = true; lastLocation = value; } }
    bool locationUpdated = true;
    bool allowPointRepath = true;
    bool allowTimerRepath = true;
    bool overrideAllowRepath = true;

    public override void SetUp(Dictionary<string, object> message)
    {
        base.SetUp(message);
        playerChar = logic.playerChar;
        repathTimer = GetNode<Timer>("Timer");
        //repathTimer.Timeout += () => UpdateChaseLocation(logic.lastSighting);

    }
    public override void OnStart(Dictionary<string, object> message)
    {
        /*statusAnimator.Play("!");
        logic.HaltPathing();
        logic.isAlerted = true;*/
        base.OnStart(message);
        animator.Play("Walk", 1.5f);
        //lastLocation = goToPoint;
        repathTimer.Start();



    }

    


    void EndChase()
    {

        if (logic.canSee)
        {
            machine.ChangeState("EnemyChaseState", null);

        }
        else
            machine.ChangeState("EnemyConfusedState", new Dictionary<string, object> { { "canSee", logic.canSee } });

    }
   
    public override void OnExit(string nextState)
    {
        base.OnExit(nextState);

        logic.isAlerted = false;
        locationUpdated = true;
        statusAnimator.Play("None");
        repathTimer.Stop();
    }
}
