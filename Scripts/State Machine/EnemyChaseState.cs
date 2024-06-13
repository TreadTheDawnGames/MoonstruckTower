using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

public partial class EnemyChaseState : EnemyState
{
    player link;
    Area2D rangeArea;
    Timer repathTimer;
    private Vector2 lastLocation;
    public Vector2 LastLocation { get { return lastLocation; } set { if (lastLocation != value) locationUpdated = true; lastLocation = value; } }
    bool chasing = false;
    bool locationUpdated = false;


    public override void SetUp(Dictionary<string, object> message)
    {
        base.SetUp(message);
        link = logic.link;
        repathTimer = GetNode<Timer>("Timer");
        repathTimer.Timeout += () => UpdateChaseLocation(logic.lastSighting);

    }
    public override void OnStart(Dictionary<string, object> message)
    {
       base.OnStart(message);
        chasing = true;
        animator.Play("Walk", 1.5f);
        statusAnimator.Play("!");
        LastLocation = logic.lastSighting;
        //lastLocation = goToPoint;
        repathTimer.Start();

        UpdateChaseLocation(lastLocation);
        logic.pathfinder.PathfindEnd += EndChase;
        logic.pathfinder.ReachedPoint += PointReached;

        GD.Print(logic.Name + " is Chasing to " + lastLocation);
    }

    void PointReached()
    {
        if (logic.pathfinder.localMapPosition != logic.pathfinder.savedTarget)
        {
            UpdateChaseLocation(lastLocation);
        }
        //queue next point
        //else
        //ExitChase
    }

    void UpdateChaseLocation(Vector2 goToPoint)
    {
        /*repathTimer.Start();
        LastLocation = goToPoint;
        if (!locationUpdated)
        {
            //machine.ChangeState("EnemyConfusedState", null);
            return;
        }*/
        locationUpdated = false;
        if (isCurrentState)
        {
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
        //LastLocation = goToPoint;
        }
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
        logic.pathfinder.ReachedPoint -= PointReached;

        chasing = false;
        statusAnimator.Play("None");
        repathTimer.Stop();
    }
}
