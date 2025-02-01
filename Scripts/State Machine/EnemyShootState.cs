using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyShootState : EnemyState
{
    ShootyPlant octorok;
    [Export]
    AudioStream shoot;
    public override void SetUp(Dictionary<string, object> message)
    {
        base.SetUp(message);

        octorok = (ShootyPlant)logic;

    }
   

    public override void OnStart(Dictionary<string, object> message)
    {
        base.OnStart(message);
        

        animator.AnimationFinished += SpawnPellet;
        
        
        logic.animator.Play("PrepShoot");

        if (octorok.shootCharged)
        {
            octorok.shootCharged = false;
            octorok.shootTimer.Start();
        }




        

    }

    


    void SpawnPellet()
    {

        if (animator.Animation == "PrepShoot")
        {
            octorok.projectileSpawnPoint.Set("position", new Vector2(-8, -1));

            bool flipped = animator.FlipH;

            var pellet = octorok.pellet.Instantiate<Projectile>();
            pellet.shootDirection = Vector2.Right;

            pellet.GlobalPosition = octorok.projectileSpawnPoint.GlobalPosition;
            pellet.GlobalRotation = flipped ? Mathf.DegToRad(0f) : Mathf.DegToRad(180f);
            pellet.speed *= flipped ? 1 : -1;

            GetTree().Root.GetChild<Node2D>(0).AddChild(pellet);
            animator.Play("EndShoot");
            audioPlayer.PlaySound(shoot);
        }
        else if (animator.Animation == "EndShoot")
        {
            machine.ChangeState("EnemyIdleState", null);
        }

    }

    public override void OnExit(string nextState)
    {
        base.OnExit(nextState);
        animator.AnimationFinished -= SpawnPellet;
        statusAnimator.Play("None");
    }

}
