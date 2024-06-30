using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyAlertState : EnemyState
{
    Timer timer;
    public override void SetUp(Dictionary<string, object> message)
    {
        base.SetUp(message);
        timer = GetNode<Timer>("Timer");
    }

    public override void OnStart(Dictionary<string, object> message)
    {
        base.OnStart(message);
        statusAnimator.Play("!");
        logic.isAlerted = true;
        timer.Timeout += SetAlertTimeout;
        timer.Start();
    }

    void SetAlertTimeout()
    {
			machine.ChangeState("EnemyPanicState", null);
    }

    public override void UpdateState(float delta)
    {
        base.UpdateState(delta);
        if (logic.IsOnFloor())
        {
            float jumpInPixels = -Mathf.Sqrt(2 * logic.gravity * logic.jumpHeightPix);

            logic.velocity.Y = jumpInPixels;

        }
    }

    public override void OnExit(string nextState)
    {
        base.OnExit(nextState);
        timer.Stop();
        timer.Timeout -= SetAlertTimeout;
        logic.isAlerted = false;
    }
}
