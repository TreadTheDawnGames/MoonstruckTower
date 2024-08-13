using Godot;
using System;

[Icon("res://Assets/Locks and Doors/Icons/ButtonIcon.png")]
public partial class ButtonLock : Lock, ILock
{

    protected Sprite2D sprite;
    Timer unpressTimer;
    bool queued = false;
    [Export] bool inverted = false;

    bool shouldPlaySound = false;
    bool lastLock;
    // Called when the node enters the scene tree for the first time.
    public override void SetUp()
    {
        audioPlayer = GetNode<AudioPlayer>("AudioStreamPlayer2D");
        sprite = GetNode<Sprite2D>("Sprite2D");
        //unpressTimer = GetNode<Timer>("Timer");
        unlocked = inverted;
        if (!inverted)
        {
            BodyEntered += UnlockMe;
            BodyExited += (node) => LockMe();

        }
        else
        {
            BodyEntered += (node) => LockMe();
            BodyExited += UnlockMe;

        }


    }
    public override void UnlockMe(Node2D node)
    {
        if (HasOverlappingBodies() && inverted)
            return;
        base.UnlockMe(node);

        if (!inverted)
        {
            sprite.Frame = 1;

        }
        else
        {
            sprite.Frame = 0;

        }
        shouldPlaySound = (lastLock != unlocked);

        if(shouldPlaySound)
        {
            audioPlayer.PlaySound(unlockedSound);
        }
        lastLock = unlocked;


    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (queued)
        {
                Lock();
            queued = false;
        }
    }

    public override void LockMe()
    {



        queued = true;
        
        
    }

    void Lock()
    {
        if (HasOverlappingBodies()&&!inverted)
            return;
        base.LockMe();
        if (!inverted)
        {
            sprite.Frame = 0;

        }
        else
        {
            sprite.Frame = 1;
        }
        shouldPlaySound = (lastLock != unlocked);
        if (shouldPlaySound)
        {
            audioPlayer.PlaySound(lockedSound);
        }
        lastLock = unlocked;
    }

}
