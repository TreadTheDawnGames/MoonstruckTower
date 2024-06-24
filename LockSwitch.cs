using Godot;
using System;
using System.Linq;

public partial class LockSwitch : Lock
{
    [Export]
    AudioStream lockSound;
    [Export]
    AudioStream unlockSound;

    protected override void UnlockMe()
    {
        unlocked = !unlocked;
        if (unlocked)
        {
            sprite.Play("Unlocked");
            audioPlayer.Stream = unlockSound;
            door.Open();
        }
        else
        {
            sprite.Play("Locked");
            audioPlayer.Stream = lockSound;
            door.Close();
        }
        audioPlayer.Play();

        
    }
}
