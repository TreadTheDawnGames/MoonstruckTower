using Godot;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using static Godot.TextServer;

public partial class Dino : EnemyBase
{


    bool walkStep1 = false;
    [Export] AudioStream walk1, walk2;

    public override void Activate()
    {
        if (animator.Animation == "Spawn")
        {
            base.Activate();
            passiveHitBox.BodyEntered += CheckConfused;
        }
    }

    public override void Destroy()
    {
        passiveHitBox.BodyEntered -= CheckConfused;

        base.Destroy();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        if (!active) return;
        base._PhysicsProcess(delta);

        //velocity = Velocity;
        if (!IsOnFloor())
        {
            velocity.Y += gravity * (float)delta;

        }

        if (!display)
        {
            var relitivePlayerCharPos = playerChar.GlobalPosition - GlobalPosition;

            if (GlobalPosition.DistanceTo(playerChar.GlobalPosition) < sightRange)
                visionCast.TargetPosition = relitivePlayerCharPos;
            else
                visionCast.TargetPosition = Vector2.Zero;

            

            if (visionCast.GetColliderRid() == playerChar.GetRid())
            {
                if (((Mathf.Sign(relitivePlayerCharPos.X) >= 0 && animator.FlipH) || (Mathf.Sign(relitivePlayerCharPos.X) <= 0 && !animator.FlipH)))
                {

                    canSee = true;
                }
                else
                {
                    canSee = false;
                }


                if (machine.CurrentState != "EnemyAlertState" && machine.CurrentState != "EnemyPanicState" && !isAlerted && canSee && !isBusy)
                {
                    machine.ChangeState("EnemyAlertState", null);
                }

            }
            else
            {
                canSee = false;
            }
        }
        
        //facing right
        if (Velocity.X > 0)
        {
            animator.FlipH = true;
            flippables.Scale = new Vector2(-1, 1);
            hitBox.damage = 1;
        }
        //facing left
        else if (Velocity.X < 0)
        {
            animator.FlipH = false;
            flippables.Scale = new Vector2(1, 1);
            hitBox.damage = -1;
        }
        facingRight = animator.FlipH;

        if (!takingDamage)
            {

                if (walkDirection != 0)
                {
                    FlipDirection();
                    velocity.X = walkSpeed * walkDirection;
                
                if (velocity.X != 0 && !audioPlayer.Playing && IsOnFloor())
                {
                    walkStep1 = !walkStep1;
                    audioPlayer.PlaySound(walkStep1 ? walk1 : walk2);
                }
            }
                else
                {
                    velocity.X = Mathf.MoveToward(Velocity.X, 0, walkSpeed);
                }
            }


       

        Velocity = velocity;
        
        
            MoveAndSlide(); 
        
        
        

    }

   
    void CheckConfused(Node2D node)
    {
        

            if (!canSee && !isBusy)
            {
                machine.ChangeState("EnemyConfusedState", null);
            }
            else if (canSee && !isBusy) 
            {
                machine.ChangeState("EnemyAlertState", null);

            }

        

    }

    
    

    
}

