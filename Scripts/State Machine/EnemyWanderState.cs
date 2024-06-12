using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyWanderState : EnemyState
{


    public override void SetUp(Dictionary<string, object> message)
    {
        base.SetUp(message);
        logic.pathfinder.PathfindEnd += ()=> EndWander();
    
    }

    

    // Called when the node enters the scene tree for the first time.
    public override void OnStart(Dictionary<string, object> message)
    {
            base.OnStart(message);



       Vector2 goTo = GetRandomLocation();
        
        while (!logic.pathfinder.CreateAndGoToValidPath(goTo))
        {
            goTo = GetRandomLocation();
            //nothing
        } 
            logic.pathfinder.mapPather.AddVisualPoint(logic.pathfinder.mapPather.ConvertPointPositionToMapPosition(goTo), new Color(0.5f,0.5f,1,1), scale: 1.5f);
            GD.Print(Owner.Name + " started wandering to " + goTo);
        
       
            animator.Play("Walk");
        
    }

    

    Vector2 GetRandomLocation()
    {
        float wanderX = GD.RandRange(-160, 160);
        float wanderY = GD.RandRange(-160, 0);


        return (logic.pathfinder.GlobalPosition + new Vector2(wanderX, wanderY)).Clamp(new Vector2(-240+16, wanderY), new Vector2(176-16, wanderY));

    }

    void EndWander()
    {
        if (isCurrentState)
        {
            GD.Print("End wander");
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
