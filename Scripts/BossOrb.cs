using Godot;
using System;
using System.ComponentModel.Design;

public partial class BossOrb : Lock
{
    [Export] int maxHitPoints = 3;
     int currentHitPoints;
    AnimatedSprite2D animator;
    string currentAnimation;
    [Export] public bool active = false;


    public override void _Ready()
    {
        base._Ready();
        animator = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animator.AnimationFinished += AnimatorEnd;
        currentHitPoints = maxHitPoints;
        if(!active)
        {
            animator.Play("Hidden");
        }
        else
        {
            animator.Play("Healthy");
        }
    }

    public void Activate()
    {
        active = true;
        unlocked = false;
        animator.Play("Healthy");
    }

    void AnimatorEnd()
    {
        if (animator.Animation == "Damaged")
        {
            if (currentHitPoints <= maxHitPoints / 3)
            {
                animator.Play("Cracked+");
            }
            else if (currentHitPoints <= 2 * (maxHitPoints / 3) || currentHitPoints <= maxHitPoints / 2)
            {
                animator.Play("Cracked");
            }
            else
            {
                animator.Play(currentAnimation);
            }
        }
    }

    protected override void UnlockMe(Node2D node)
    {
        GD.Print("Attempting to unlock " + Name + " with " + node.Name);
        if (unlocked || animator.Animation == "Hidden")
        {
            return;
        }
        currentAnimation = animator.Animation;
        if (node.Owner is Player || node.Owner is Arrow)
        {
            if(node.Owner is Arrow)
            {
                Arrow arrow = (Arrow)node.Owner;
                arrow.HitHurtBox();
            }
            currentHitPoints--;
            if (currentHitPoints <= 0)
            {
                animator.Play("Explode");
                unlocked = true;
                door.AttemptToOpen();
            }
            else
            {
                animator.Play("Damaged");

            }

        }

    }

    public override void LockMe()
    {
        base.LockMe();
        
        currentHitPoints = maxHitPoints;
        
            animator.Play("Hidden");
        
    }




}
