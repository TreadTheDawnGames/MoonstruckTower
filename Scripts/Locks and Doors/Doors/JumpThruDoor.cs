using Godot;
using System;

public partial class JumpThruDoor : Door
{
	Sprite2D sprite;
    CollisionShape2D collisionShape;
    StaticBody2D staticBody;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("Sprite2D");
		collisionShape = GetNode<CollisionShape2D>("StaticBody2D/CollisionShape2D");
		staticBody = GetNode<StaticBody2D>("StaticBody2D");
        if (!staticBody.GetCollisionLayerValue(1))
        {
            staticBody.SetCollisionLayerValue(1, true);
        }
		base._Ready();
    }

    public override bool AttemptToOpen()
    {
		if (!base.AttemptToOpen())
		{
			return false;
		}
		if (opened)
		{

			if (staticBody.GetCollisionLayerValue(1))
			{
			//	staticBody.SetCollisionLayerValue(1, false);
			}

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
            if (!staticBody.GetCollisionLayerValue(1))
            {
               // staticBody.SetCollisionLayerValue(1, true);
            }
            sprite.Frame = 0;
			collisionShape.SetDeferred("one_way_collision", false);
        }
		return true;
    }
}
