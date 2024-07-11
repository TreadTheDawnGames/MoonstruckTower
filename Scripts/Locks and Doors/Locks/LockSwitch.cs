using Godot;
using System;
using System.Linq;

public partial class LockSwitch : Lock
{
    [Export] bool inverted = false;
    bool visualUnlocked = false;
    AnimatedSprite2D sprite;
    public override void _Ready()
    {
        sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        base._Ready();
        if (inverted)
        {
            unlocked = true;
        }
        visualUnlocked = false;
    }

    public override void UnlockMe(Node2D node)
    {


        if (node.Owner is Player)
        {
            //get arrow owner script /detect if box was on projectile and if it was, delete it
            Player link = node.Owner.GetNode<Player>(node.Owner.GetPath());

            if (link != null && link.flippedSwitchThisAnimation)
            {
                return;
            }

            link.flippedSwitchThisAnimation = true;

        }


        unlocked = !unlocked;
        visualUnlocked = !visualUnlocked;
        sprite.Play(visualUnlocked ? "Unlocked" : "Locked");


        //this is bugging the WallDoor

       
            door.AttemptToOpen();
        
        
        

    }
}
