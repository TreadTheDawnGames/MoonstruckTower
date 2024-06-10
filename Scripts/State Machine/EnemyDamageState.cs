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
        int damage = (int)message["damage"];
        logic.hitPoints -= damage;
        logic.takingDamage = true;
        //body.takingDamage = true;
        if (logic.hitPoints <= 0)
        {

           // logic.hurtBox.SetDeferred("disabled", true);
           // logic.collisionBox.SetDeferred("disabled", true);
           // animator.Stop();
            animator.Play("Death");

        }
        else
        {
            animator.Play("Damage");

        }
    }

    void Destroy()
    {

        logic.takingDamage = false;
            if (animator.Animation == "Death")
            {
                logic.pathfinder.Destroy ();
            }
            else if (animator.Animation == "Damage")
            {
                if (!logic.canSee)
                {

                    machine.ChangeState("EnemyConfusedState", null);
                }
                else { machine.ChangeState("EnemyChaseState", new Dictionary<string, object> { { "goToPoint", logic.lastSighting } }); }
            }

        
    }
}
