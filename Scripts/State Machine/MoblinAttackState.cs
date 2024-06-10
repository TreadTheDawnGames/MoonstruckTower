using Godot;
using System;
using System.Collections.Generic;

public partial class MoblinAttackState : EnemyAttackState
{
    HitBox2D hitBox;
    CollisionShape2D hitboxShape;
    Timer timer;
    
    public override void SetUp(Dictionary<string, object> message)
    {
        base.SetUp(message);
        hitBox = Owner.GetNode<HitBox2D>("Flippables/HitBox2D");
        timer = GetNode<Timer>("Timer");
        timer.Timeout += () => EndAttack();
    }
    public override void OnStart(Dictionary<string, object> message)
    {
        GD.Print(logic.Name + " is Attacking");
        animator.Stop();
        animator.Play("Attack");
        base.OnStart(message);
        hitBox.SetEnabled(true);
        timer.Start();

    }

    void EndAttack()
    {
        machine.ChangeState("EnemyIdleState", null);
    }

    public override void OnExit(string nextState)
    {
        base.OnExit(nextState);
        hitBox.SetEnabled(false);
        timer.Stop();
    }
    
}
