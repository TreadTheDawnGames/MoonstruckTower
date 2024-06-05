using Godot;
using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

public partial class EnemyDamageState : EnemyState
{
    public override void SetUp(Dictionary<string, object> message)
    {
        base.SetUp(message);
        animator.AnimationFinished += () => Destroy();

    }


    public override void OnStart(Dictionary<string, object> message)
    {
        base.OnStart(message);
        GD.Print(Owner.Name + " is being damaged");
        body.hitPoints -= (int)message["damage"];

        //body.takingDamage = true;
        if (body.hitPoints <= 0)
        {

            body.hurtBox.SetDeferred("disabled", true);
            body.collisionBox.SetDeferred("disabled", true);

            animator.Play("Death");

            return;
        }
        animator.Play("Damage");
    }

    void Destroy()
    {
        if (animator.Animation == "Death")
        {
            body.Destroy();
        }
        else
        {
            if (!body.hasTarget)
            {

                machine.ChangeState("EnemyConfusedState", null);
            }
            else { machine.ChangeState("EnemyChaseState", null); }
        }

    }
}
