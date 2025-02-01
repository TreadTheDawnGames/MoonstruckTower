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
        logic.isBusy = true;
        base.OnStart(message);
        panicTimes = 0;

        panicRepeats = (int)GD.RandRange(5, 9);
        statusAnimator.Play("!");
        animator.Play("PanicAttack", 1.5f);
        timer.Timeout += StopPanicking;

        StartPanic();
        logic.hitBox.SetEnabled(true);
        logic.isBusy = true;
    }

    bool StartPanic()
    {

        panicTimes++;

        if (panicTimes >= panicRepeats)
        {
            return false;
        }


        /* if (logic.animator.FlipH)
         {
             logic.walkDirection = -1;
         }
         else
         {
             logic.walkDirection = 1;
         }*/
        
                do
                {
                    logic.walkDirection = Mathf.Sign(GD.RandRange(-1, 1));
                }
                while (logic.walkDirection == 0);
        

        if (!logic.IsOnFloor())
            {
                logic.walkDirection = 0;
                queueStartTravel = true;
            }

            logic.walkSpeed = 1.5f * logic.baseWalkSpeed;

            timer.WaitTime = GD.RandRange(0.5f, 0.75f);

            timer.Start();


        return true;
    }

    public override void UpdateState(float delta)
    {
        base.UpdateState(delta);

        //FlipDirection();


        if (queueStartTravel && logic.IsOnFloor())
        {
            queueStartTravel = false;
            StartPanic();
        }
    }

    public void StopPanicking()
    {
        if (logic.IsOnFloor())
        {

            
            
            //machine.ChangeState("EnemyPanicState", new Dictionary<string, object> { { "repeat", panicTimes } });
            if (!StartPanic())
            {
                logic.isAlerted = false;
                logic.walkDirection = 0;

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
        timer.Timeout -= StopPanicking;
        logic.isBusy = false;
        logic.hitBox.SetEnabled(false);
        logic.isBusy = false;
    }

}