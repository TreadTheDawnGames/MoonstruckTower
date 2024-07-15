using Godot;
using System;
using System.Linq;

[Icon("res://Assets/Locks and Doors/Icons/SwitchIcon.png")]
public partial class LockSwitch : Lock
{
    [Export] bool inverted = false;
    bool visualUnlocked = false;
    AnimatedSprite2D sprite;
    public override void _Ready()
    {
        sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        base._Ready();
         
        unlocked = inverted;
        

        visualUnlocked = false;
        sprite.Play(visualUnlocked ? "Unlocked" : "Locked");
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }

    public override void UnlockMe(Node2D node)
    {

       

        if (node.Owner is Player || node.Owner is Projectile)
        {
            if (node.Owner is Projectile)
            {
                //get arrow owner script /detect if box was on projectile and if it was, delete it

                Projectile arrow = node.Owner.GetNode<Projectile>(node.Owner.GetPath());

                arrow.HitHurtBox();

            }
            else if (node.Owner is Player) 
            {

                //get arrow owner script /detect if box was on projectile and if it was, delete it
                Player link = node.Owner.GetNode<Player>(node.Owner.GetPath());

                if (link != null && link.flippedSwitchThisAnimation)
                {
                    return;
                }

                link.flippedSwitchThisAnimation = true;
            }

        }

 unlocked = !unlocked;
        visualUnlocked = !visualUnlocked;
        sprite.Play(visualUnlocked ? "Unlocked" : "Locked");



       
            door.AttemptToOpen();
        
        
        

    }
}
