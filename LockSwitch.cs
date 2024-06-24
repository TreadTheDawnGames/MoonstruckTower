using Godot;
using System;
using System.Linq;

public partial class LockSwitch : Lock
{
       

    protected override void UnlockMe()
    {
        unlocked = !unlocked;
        if (unlocked)
        {
            sprite.Play("Unlocked");
            door.Open();
        }
        else
        {
            sprite.Play("Locked");
            door.Close();
        }

        
    }
}
