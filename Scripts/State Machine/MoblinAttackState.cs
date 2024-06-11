using Godot;
using System;
using System.Collections.Generic;

public partial class MoblinAttackState : EnemyAttackState
{
    
    public override void SetUp(Dictionary<string, object> message)
    {
        base.SetUp(message);
        animator.AnimationFinished += () => EndAttack();
    }
    public override void OnStart(Dictionary<string, object> message)
    {
        GD.Print(logic.Name + " is Attacking");
        animator.Stop();
        animator.Play("Attack");
        base.OnStart(message);
        logic.hitBox.SetEnabled(true);
        logic.isBusy = true;

    }

    void EndAttack()
    {
        if(animator.Animation == "Attack")  
            machine.ChangeState("EnemyIdleState", null);
    }

    public override void OnExit(string nextState)
    {
        base.OnExit(nextState);
        logic.hitBox.SetEnabled(false);
        logic.isBusy = false;
    }
    
}
