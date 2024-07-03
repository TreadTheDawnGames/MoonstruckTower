using Godot;
using System;
using System.Collections.Generic;

public partial class MoblinAttackState : EnemyAttackState
{
    
    public override void SetUp(Dictionary<string, object> message)
    {
        base.SetUp(message);
    }
   
    public override void OnStart(Dictionary<string, object> message)
    {
        animator.AnimationFinished +=  EndAttack;
        animator.Stop();
        animator.Play("Attack");
        base.OnStart(message);
        logic.hitBox.SetEnabled(true);
        logic.isBusy = true;

    }

    void EndAttack()
    {
        if(animator.Animation == "Attack")
        {
            if (logic.canSee)
            {
                machine.ChangeState("EnemyAlertState", null);
            }
            else
            {
                machine.ChangeState("EnemyIdleState", null);
            }
        }
    }

    public override void OnExit(string nextState)
    {
        base.OnExit(nextState);
        animator.AnimationFinished -= EndAttack;
        logic.hitBox.SetEnabled(false);
        logic.isBusy = false;
        logic.isAlerted = false;
    }
    
}
