using Godot;
using System;
using System.Collections.Generic;

public partial class OctorokV2 : EnemyBase
{
    public Timer shootTimer;
    public bool shootCharged = true;
    public PackedScene pellet = GD.Load<PackedScene>("res://Scenes/Enemies/octo_pellet.tscn");
    public Marker2D projectileSpawnPoint;

    public override void _Ready()
    {
        base._Ready();
        projectileSpawnPoint = GetNode<Marker2D>("Flippables/Marker2D");
        shootTimer = GetNode<Timer>("ShootTimer");
        //passiveHitBox.BodyEntered += (node) => CheckConfused(node);
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


        var relitiveLinkPos = link.GlobalPosition - GlobalPosition;

        if (GlobalPosition.DistanceTo(link.GlobalPosition) < sightRange)
            visionCast.TargetPosition = relitiveLinkPos;
        else
            visionCast.TargetPosition = Vector2.Zero;

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

        if (visionCast.GetColliderRid() == link.GetRid())
        {
            if (((Mathf.Sign(relitiveLinkPos.X) >= 0 && animator.FlipH) || (Mathf.Sign(relitiveLinkPos.X) <= 0 && !animator.FlipH)))
            {
                canSee = true;
            }
            else
            {

                canSee = false;
            }


            if (machine.CurrentState != "EnemyShootState" && !isAlerted && canSee && !isBusy && link.GlobalPosition.Y-16<=GlobalPosition.Y)
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

