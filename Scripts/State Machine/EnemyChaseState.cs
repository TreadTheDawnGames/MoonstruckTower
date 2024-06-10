using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyChaseState : EnemyState
{
    player link;
    Area2D rangeArea;
    Timer repathTimer;
    Vector2 lastLocation;
    bool chasing = false;

    public override void SetUp(Dictionary<string, object> message)
    {
        base.SetUp(message);
        link = logic.link;
        repathTimer = GetNode<Timer>("Timer");
        logic.pathfinder.PathfindEnd += ()=> EndChase();
        logic.pathfinder.UnreachablePoint += ()=> UnreachablePoint(lastLocation);
        repathTimer.Timeout += ()=> UpdateChaseLocation(lastLocation);

    }
    public override void OnStart(Dictionary<string, object> message)
    {
       base.OnStart(message);
        chasing = true;
        animator.Play("Walk", 1.5f);
        statusAnimator.Play("!");
        var goToPoint = (Vector2)message["goToPoint"];
        lastLocation = goToPoint;

        UpdateChaseLocation(lastLocation);

        GD.Print(logic.Name + " is Chasing to " + lastLocation);
    }

    void UpdateChaseLocation(Vector2 goToPoint)
    {
        
        
            goToPoint.Y-=16;
            logic.pathfinder.CreateAndGoToPath(goToPoint);
        
        repathTimer.Start();
        lastLocation = logic.lastSighting;
    }

    void UnreachablePoint(Vector2 goToPoint)
    {
        
    }

    void EndChase()
    {
        if (isCurrentState)
        {
            if (chasing)
            {
                chasing = false;
                logic.pathfinder.CreateAndGoToPath(lastLocation);
            }
            else
            {

                GD.Print("End Chase");
                if (logic.canSee)
                {
                    machine.ChangeState("EnemyIdleState", new System.Collections.Generic.Dictionary<string, object> { { "goToPoint", logic.link.Position } });

                }
                else
                    machine.ChangeState("EnemyConfusedState", new Dictionary<string, object> { { "canSee", logic.canSee } });
            }
        }
    }
   
    public override void OnExit(string nextState)
    {
        base.OnExit(nextState);

        chasing = false;
        statusAnimator.Play("None");
        repathTimer.Stop();
    }
}
