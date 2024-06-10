using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyWanderState : EnemyState
{
    


    public override void SetUp(Dictionary<string, object> message)
    {
        base.SetUp(message);
        logic.pathfinder.PathfindEnd += ()=> EndWander();
        logic.pathfinder.UnreachablePoint += ()=> UnreachablePoint();
    
    }

    // Called when the node enters the scene tree for the first time.
    public override void OnStart(Dictionary<string, object> message)
    {
       

            base.OnStart(message);
            animator.Play("Walk");
            float wanderX = GD.RandRange(-160, 160);
            float wanderY = GD.RandRange(-160, 0);
            Vector2 goTo = new Vector2(wanderX, wanderY);
            GD.Print(Owner.Name + " started wandering to " + goTo);

            logic.pathfinder.CreateAndGoToPath(goTo);
        
    }

    void EndWander()
    {
        if (isCurrentState)
        {
            GD.Print("End wander");
            machine.ChangeState("EnemyIdleState", null);
        }
    }
    void UnreachablePoint()
    {
        if (isCurrentState)
        {
            animator.FlipH = !animator.FlipH;
            GD.Print("Attempted to go to unreachable point");
            machine.ChangeState("EnemyIdleState", null);
        }
    }

    public override void UpdateState(float delta)
    {
        //moveDirection = wanderDirection;
        base.UpdateState(delta);


    }

    public override void OnExit(string nextState)
    {
        base.OnExit(nextState);
    }

}
