using Godot;
using System;

public partial class LockEye : Lock
{
	AnimatedSprite2D sprite;
	[Export] bool inverted = false;
    public override void _Ready()
    {
        sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        shape = GetNode<CollisionShape2D>("CollisionShape2D");

		if(!inverted )
		{
	        AreaEntered += (node) => UnlockMe(node);
		}

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
