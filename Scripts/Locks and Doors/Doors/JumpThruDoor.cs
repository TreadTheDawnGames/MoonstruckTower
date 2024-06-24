using Godot;
using System;

public partial class JumpThruDoor : Door
{
	Sprite2D sprite;
	CollisionShape2D collisionShape;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		sprite = GetNode<Sprite2D>("Sprite2D");
		collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");

	}

    public override bool Open()
    {

		if (!base.Open())
		{
			return false;
		}
		if (opened)
		{

			sprite.Frame = 1;
			collisionShape.SetDeferred("one_way_collision", true);
		}
		return true;
    }

    public override bool Close()
    {
		if (!base.Close())
		{
			return false;
		}

		if(!opened)
		{
			sprite.Frame = 0;
			collisionShape.SetDeferred("one_way_collision", false);
        }
		return true;
    }
}
