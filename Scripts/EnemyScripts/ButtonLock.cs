using Godot;
using System;

public partial class ButtonLock : Lock
{

    Sprite2D sprite;
    Timer unpressTimer;
    bool queued = false;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        sprite = GetNode<Sprite2D>("Sprite2D");
        unpressTimer = GetNode<Timer>("Timer");
        BodyEntered += UnlockMe;
        BodyExited += (node) => LockMe();
    }
    protected override void UnlockMe(Node2D node)
    {
        
            base.UnlockMe(node);
            sprite.Frame = 1;

        
        
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
        GD.Print(HasOverlappingBodies());
        if (HasOverlappingBodies())
            return;
        base.LockMe();
        sprite.Frame = 0;
    }

}
