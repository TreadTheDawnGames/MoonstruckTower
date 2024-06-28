 using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyPanicState : EnemyState
{


    Timer timer;
    int panicRepeats;
    int panicTimes;

    bool queueStartTravel = false;

    public override void SetUp(Dictionary<string, object> message)
    {
        base.SetUp(message);


        timer = GetNode<Timer>("Timer");
    }

    // Called when the node enters the scene tree for the first time.
    public override void OnStart(Dictionary<string, object> message)
    {
        panicTimes = (int)message["repeat"];

        base.OnStart(message);

        panicRepeats = (int)GD.RandRange(5, 9);

        animator.Play("Walk", 1.5f);
        timer.Timeout += StopWandering;

        StartTravel();

    }

    void StartTravel()
    {

        
        

            if (logic.animator.FlipH)
            {
                logic.walkDirection = -1;
            }
            else
            {
                logic.walkDirection = 1;
            }
            if (!logic.IsOnFloor())
            {
                logic.walkDirection = 0;
                queueStartTravel = true;
            }

            logic.walkSpeed = 1.5f * logic.baseWalkSpeed;

            timer.WaitTime = GD.RandRange(0.5f, 0.75f);
            GD.Print(logic.Name + " is Wandering for " + timer.WaitTime + " seconds");

            timer.Start();

        

    }

    public override void UpdateState(float delta)
    {
        base.UpdateState(delta);
        
            //FlipDirection();


        if (queueStartTravel&&logic.IsOnFloor())
        {
            queueStartTravel = false;
            StartTravel();
        }
    }

    public void StopWandering()
    {
        if (logic.IsOnFloor())
        {
            
                panicTimes++;
            

            if (panicTimes <= panicRepeats)
            {
//                machine.ChangeState("EnemyPanicState", new Dictionary<string, object> { { "repeat", panicTimes } });
                StartTravel();
            }
            else
            {
                logic.isAlerted = false;
                logic.walkDirection = 0;

                GD.Print("End wander");
                machine.ChangeState("EnemyIdleState", null);
            }

        }


    }
    



    public override void OnExit(string nextState)
    {
        base.OnExit(nextState);
        timer.Stop();
        logic.walkDirection = 0;
        logic.walkSpeed = logic.baseWalkSpeed;
        panicTimes = 0;
        timer.Timeout -= StopWandering;

    }

}