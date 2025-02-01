using Godot;
using System;

public partial class ShootyPlantPellet : Projectile
{
    AnimatedSprite2D animatedSprite;
    public override void _Ready()
    {
        base._Ready();
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animatedSprite.AnimationFinished += FinishDestroyAnimation;
    }

    protected override void HitWorld(Node2D node)
    {
        
        base.HitWorld(node);

        animatedSprite.Play("Destroy");    
        
    }

    public override void HitHurtBox()
    {
        Velocity = Vector2.Zero;
        speed = 0;
        animatedSprite.Play("Destroy");
    }

    void FinishDestroyAnimation()
    {
        if(animatedSprite.Animation=="Destroy")
            QueueFree();
    }
}
