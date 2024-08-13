using Godot;
using System;
using System.Linq;

[Icon("res://Assets/Locks and Doors/Icons/SwitchIcon.png")]
public partial class LockSwitch : Lock
{
    [Export] bool inverted = false;
    bool visualUnlocked = false;
    AnimatedSprite2D sprite;

    public override void SetUp()
    {
        base.SetUp();
        sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
         
           unlocked = inverted;
        base._Ready();


        //sprite.Play("Locked");
    }

 

    public override void UnlockMe(Node2D node)
    {
        if (HasOverlappingBodies())
        {

            if (!GetOverlappingBodies().OfType<Player>().Any())
            {
                return;
            }
        }

        if (node != null)
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
        }

        unlocked = !unlocked;
        visualUnlocked = !visualUnlocked;

        

            sprite.Play(visualUnlocked ? "Unlocked" : "Locked");
        audioPlayer.PlaySound(visualUnlocked ? lockedSound : unlockedSound);



            door.AttemptToOpen();
/*
        try
        {

        }
        catch (Exception ex)
        {
            GD.PrintErr(Name + " has no assigned door" + " | " + Owner.Name);
        }*/
        
        

    }
}
