using Godot;
using System;

public partial class ButtonLock : Lock
{

    protected Sprite2D sprite;
    Timer unpressTimer;
    bool queued = false;
    [Export] bool inverted = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        sprite = GetNode<Sprite2D>("Sprite2D");
        unpressTimer = GetNode<Timer>("Timer");
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
    protected override void UnlockMe(Node2D node)
    {
        if (HasOverlappingBodies() && inverted)
            return;
        base.UnlockMe(node);
        GD.Print("Unlocked");

        if (!inverted)
        {
            GD.Print("Sprite 1");
            sprite.Frame = 1;
        }
        else
        {
            GD.Print("Sprite 0");
            sprite.Frame = 0;
        }




    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (queued)
        {
                GD.Print("Locked");
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
            GD.Print("Sprite 0");
            sprite.Frame = 0;
        }
        else
        {
            GD.Print("Sprite 1");
            sprite.Frame = 1;
        }
    }

}
