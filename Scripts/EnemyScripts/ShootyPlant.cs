using Godot;
using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;

public partial class ShootyPlant : EnemyBase
{
    public Timer shootTimer;
    public bool shootCharged = true;
    public PackedScene pellet = GD.Load<PackedScene>("res://Scenes/Enemies/shooty_plant_pellet.tscn");
    public Marker2D projectileSpawnPoint;

    public override void _Ready()
    {
        base._Ready();
        projectileSpawnPoint = GetNode<Marker2D>("Flippables/Marker2D");
        shootTimer = GetNode<Timer>("ShootTimer");
        passiveHitBox.BodyEntered += CheckConfused;

        
    }
    void CheckConfused(Node2D node)
    {


        if (!canSee && !isBusy)
        {
            machine.ChangeState("EnemyConfusedState", null);
        }
        else if (canSee && !isBusy)
        {
            if (shootCharged)
                machine.ChangeState("EnemyShootState", null);
            else
                machine.ChangeState("EnemyIdleState", null);
        }



    }
    public override void Activate()
    {
        if (animator.Animation == "Spawn")
        {
            shootTimer.Timeout += ChargePellet;

        }
        base.Activate();
    }
    void ChargePellet()
    {
        //GD.Print("Charged");
        shootCharged = true;
    }


    public override void Destroy()
    {
        passiveHitBox.BodyEntered -= CheckConfused;

        base.Destroy();
    }
    public override void _PhysicsProcess(double delta)
    {
        if (!active) return;
        base._PhysicsProcess(delta);

        //velocity = Velocity;
        if (!IsOnFloor())
        {
            velocity.Y += gravity * (float)delta;

        }


        var relitivePlayerCharPos = playerChar.GlobalPosition - GlobalPosition;

        if (GlobalPosition.DistanceTo(playerChar.GlobalPosition) < sightRange)
            visionCast.TargetPosition = relitivePlayerCharPos;
        else
            visionCast.TargetPosition = Vector2.Zero;

        //facing right
        if (Velocity.X > 0)
        {
            animator.FlipH = true;
            hitBox.damage = 1;
        }
        //facing left
        else if (Velocity.X < 0)
        {
            animator.FlipH = false;
            hitBox.damage = -1;
        }

        if(animator.FlipH)
        {
            flippables.Scale = new Vector2(-1, 1);

        }
        else
        {
            flippables.Scale = new Vector2(1, 1);

        }

        facingRight = animator.FlipH;

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


            if (machine.CurrentState != "EnemyShootState" && !isAlerted && canSee && !isBusy && playerChar.GlobalPosition.Y-16<=GlobalPosition.Y)
            {
                if (shootTimer.TimeLeft < 0.25f)
                {
                    if (statusAnimator.Animation != "!")
                    {
                        statusAnimator.Play("!");

                    }
                }
                if (shootCharged)
                {
                    machine.ChangeState("EnemyShootState", null);
                }
                

            }



        }
        else
        {
            canSee = false;
        }


        if (!takingDamage)
        {

            if (walkDirection != 0)
            {
                FlipDirection();
                velocity.X = walkSpeed * walkDirection;
            }
            else
            {
                velocity.X = Mathf.MoveToward(Velocity.X, 0, walkSpeed);
            }
        }



        Velocity = velocity;


        MoveAndSlide();

        

    }
}

