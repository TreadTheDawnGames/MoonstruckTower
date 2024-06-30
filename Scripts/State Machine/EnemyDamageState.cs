using Godot;
using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

public partial class EnemyDamageState : EnemyState
{
    [Export] float knockbackAmount = 75f;
    [Export] float damageWaitTime = 0.5f;

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
        HitBox2D caller = (HitBox2D)message["hitBox"];
        
        logic.hitPoints -= damage;
        logic.isBusy = true;
        logic.takingDamage = true;
       
        //body.takingDamage = true;
        statusAnimator.Play("None");

        if (logic.hitPoints <= 0)
        {
            logic.velocity.X = 0;
            animator.Play("Death");
        }
        else
        {
            var direction = Mathf.Sign(logic.GlobalPosition.X - caller.GlobalPosition.X);

            animator.Play("Damage");
            var hitVelX = direction * knockbackAmount * 2;
            var hitVelY = GD.RandRange(6, 12);
            float jumpInPixels = -Mathf.Sqrt(2 * logic.gravity * hitVelY);

            //logic.velocity.Y = jumpInPixels;



            logic.velocity = new Vector2(hitVelX, jumpInPixels);


        }
    }

    public override void UpdateState(float delta)
    {
        base.UpdateState(delta);
        if (logic.IsOnFloor())
        {
            logic.velocity.X = 0;
        }
    }

    void Destroy()
    {

            if (animator.Animation == "Death")
            {
                logic.Destroy ();
            }
            else if (animator.Animation == "Damage")
            {
                if (!logic.canSee)
                {

                    machine.ChangeState("EnemyConfusedState", null);
                }
                else { machine.ChangeState("EnemyAlertState", null); }
            }

        
    }

    public override void OnExit(string nextState)
    {
        base.OnExit(nextState);
        logic.isBusy = false;
        logic.takingDamage = false;

    }
}
