using Godot;
using System;
using System.Linq;

public partial class LockSwitch : Lock
{
    [Export] bool inverted = false;
    bool visualUnlocked = false;

    public override void _Ready()
    {
        base._Ready();
        if (inverted)
        {
            unlocked = true;
        }
        visualUnlocked = false;
    }

    protected override void UnlockMe()
    {

            unlocked = !unlocked;
        visualUnlocked = !visualUnlocked;
            sprite.Play(visualUnlocked ? "Unlocked" : "Locked");


        
                door.AttemptToClose();

            if (unlocked)
            {
                door.AttemptToOpen();
            }
            
        




    }
}
