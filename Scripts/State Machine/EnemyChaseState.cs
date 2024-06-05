using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyChaseState : EnemyState
{
    player link;
    Area2D rangeArea;
    public override void SetUp(Dictionary<string, object> message)
    {
        base.SetUp(message);
        link = machine.link;
        rangeArea = GetNode<Area2D>(Owner.GetPath() + "/Flippables/InRange");
        body.fov.BodyExited += (exitingBody) => TargetLost(exitingBody);
    }
    public override void OnStart(Dictionary<string, object> message)
    {
        base.OnStart(message);
        
        GD.Print(body.Name + " is Chasing");
        animator.Play("Walk", 1.5f);
        statusAnimator.Play("!");

        speedMultiplier = 1.5f;
        
    }

    

    public override void UpdateState(float delta)
    {


        Vector2 direction = link.GlobalPosition - body.GlobalPosition;
        if (direction.Y > 0 && body.floorCheck.GetOverlappingBodies().Count == 0)
        {
            jumping = true;
            body.GlobalPosition += new Vector2(0, 2);
        }
        
        GD.Print("Jumping: " + jumping);

        if (rangeArea.OverlapsBody(link))
        {
            machine.ChangeState("MoblinAttackState", null);
        }
        if(!(direction.X > -5f && direction.X < 5f))
        {
            moveDirection = Mathf.Sign(direction.X);
        }
        else
        {
            moveDirection = 0;
        }
        base.UpdateState(delta);
        
    }
    void TargetLost(Node2D node)
    {
        GD.Print("TargetLost");
        body.hasTarget = true;
        if (node == link)
        {
            machine.ChangeState("EnemyConfusedState", null);            
        }
    }
    public override void OnExit(string nextState)
    {
        base.OnExit(nextState);
        speedMultiplier = 1f;
        statusAnimator.Play("None");
    }
}
