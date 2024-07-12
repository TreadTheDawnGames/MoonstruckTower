using Godot;
using System;
using System.Reflection.Emit;

[Icon("res://Assets/Locks and Doors/Icons/JumpThruIcon.png")]
public partial class JumpThruDoor : Door
{
	Sprite2D sprite;
    CollisionShape2D collisionShape;
    StaticBody2D staticBody;
	[Export] bool inverted = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("Sprite2D");
		collisionShape = GetNode<CollisionShape2D>("StaticBody2D/CollisionShape2D");
		staticBody = GetNode<StaticBody2D>("StaticBody2D");
        
        if (inverted)
        {
            foreach (Lock locke in lockList)
            {
                locke.unlocked = true;
            }
            Close();
            foreach (Lock locke in lockList)
            {
                locke.unlocked = false;
            }
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
            ToggleCollision();
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
            ToggleCollision();
        }
		return true;

    }

    void ToggleCollision()
    {

        if (inverted)
        {
                staticBody.SetCollisionLayerValue(1, opened);
            collisionShape.SetDeferred("one_way_collision", !opened);
            sprite.Frame = opened ? 0 : 1;
            
        }
        else
        {
                staticBody.SetCollisionLayerValue(1, !opened);
            
            collisionShape.SetDeferred("one_way_collision", opened);
            sprite.Frame = opened ? 1 : 0;
        }


    }
}
