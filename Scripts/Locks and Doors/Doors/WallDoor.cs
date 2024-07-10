using Godot;
using System;

public partial class WallDoor : Door
{
    AnimationPlayer animator;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        animator = GetNode<AnimationPlayer>("AnimationPlayer");
	}

    public override bool AttemptToOpen()
    {
        if(!base.AttemptToOpen())
        {
            return false;
        }

        if(opened)
        {
            animator.Play("Open");
        }

        return true;
    }

    public override bool AttemptToClose()
    {
        if (!base.AttemptToClose())
        {
            return false;
        }

        if (!opened)
        {
            animator.Play("Open", customSpeed: -1, fromEnd: true);
        }

        return true;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
