using Godot;
using System;

[Icon("res://Assets/Locks and Doors/Icons/WallDoorIcon.png")]
public partial class WallDoor : Door
{
    AnimationPlayer animator;

    bool animationClosed = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        animator = GetNode<AnimationPlayer>("AnimationPlayer");
        base._Ready();
	}

    public override bool AttemptToOpen()
    {
        if(!base.AttemptToOpen())
        {
            return false;
        }

        if(opened && animationClosed)
        {
            animator.Play("Open");
            animationClosed = false;

        }

        return true;
    }

    public override bool Close()
    {
        if (!base.Close())
        {
            return false;
        }

        if (!opened && !animationClosed)
        {

            animator.Play("Open", customSpeed: -1, fromEnd: true);
            animationClosed = true;
        }

        return true;
    }

    
}
