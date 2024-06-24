using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyChaseState : EnemyState
{
    player link;
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
        link = logic.link;
        repathTimer = GetNode<Timer>("Timer");
        //repathTimer.Timeout += () => UpdateChaseLocation(logic.lastSighting);

    }
    public override void OnStart(Dictionary<string, object> message)
    {
        /*statusAnimator.Play("!");
        logic.pathfinder.HaltPathing();
        logic.isAlerted = true;*/
        base.OnStart(message);
        AllowTimerRepath();
        AllowPositionRepath();
        animator.Play("Walk", 1.5f);
        LastLocation = logic.lastSighting;
        //lastLocation = goToPoint;
        repathTimer.Start();

        UpdateChaseLocation(lastLocation);
        logic.pathfinder.PathfindEnd += EndChase;
        logic.pathfinder.ReachedPoint += AllowPositionRepath;
        logic.pathfinder.UnableToReachPoint+= OverrideAllowRepath;
        repathTimer.Timeout += AllowTimerRepath;

        GD.Print(logic.Name + " is Chasing to " + lastLocation);

    }

    void AllowPositionRepath()
    {
        allowPointRepath = true;
    }

    void AllowTimerRepath()
    {
        allowTimerRepath = true;
    }

    void OverrideAllowRepath()
    {
        overrideAllowRepath = true;
    }

    public override void UpdateState(float delta)
    {
        base.UpdateState(delta);
        if(allowPointRepath && allowTimerRepath || overrideAllowRepath  /*|| (allowTimerRepath && logic.pathfinder.IsOnFloor())*/)
        {
            UpdateChaseLocation(lastLocation);
            allowPointRepath=false;
            allowTimerRepath = false;
            overrideAllowRepath = false;
        }
    }

    void UpdateChaseLocation(Vector2 goToPoint)
    {
        // if inRangeofcurrentTargetpoint 
        repathTimer.Start();
        LastLocation = goToPoint;
        if (!locationUpdated)
        {
            //machine.ChangeState("EnemyConfusedState", null);
            return;
        }
        locationUpdated = false;
            //goToPoint.Y -= 16;
            var attemps = -1;
            do
            {
                if (!logic.pathfinder.CreateAndGoToValidPath(goToPoint))
                {
                    attemps++;
                }
                else
                {
                    break;
                }
                //goToPoint.Y++;

            }
            while (attemps <= 12);
            //   repathTimer.Start();

            if (attemps > 0) { GD.PrintErr("Failed find valid chase path"); }
    }


    void EndChase()
    {

        GD.Print("End Chase");
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
        logic.pathfinder.PathfindEnd -= EndChase;
        logic.pathfinder.ReachedPoint -= AllowPositionRepath;
        logic.pathfinder.UnableToReachPoint -= OverrideAllowRepath;

        repathTimer.Timeout -= AllowTimerRepath;

        logic.isAlerted = false;
        locationUpdated = true;
        statusAnimator.Play("None");
        repathTimer.Stop();
    }
}
