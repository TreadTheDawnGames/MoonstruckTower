using Godot;
using System;

public partial class LockEye : Lock
{
	AnimatedSprite2D sprite;
    public override void _Ready()
    {
        base._Ready();
        sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

    }
    protected override void UnlockMe(Node2D node)
	{
		if (!unlocked)
		{
			base.UnlockMe(node);

			sprite.Play("Close");
			door.AttemptToOpen();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
