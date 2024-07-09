using Godot;
using System;
using Godot.Collections;

public partial class DoorFallThrough : Door
{
    Sprite2D sprite;
    CollisionShape2D collisionShape;
    StaticBody2D characterBody;
    [Export] int layer;
    [Export] bool inverted = false;
   

    public override void _Ready()
    {
        base._Ready();
        sprite = GetNode<Sprite2D>("Sprite2D");
        collisionShape = GetNode<CollisionShape2D>("StaticBody2D/CollisionShape2D");
        characterBody = GetNode<StaticBody2D>("StaticBody2D");
        if (!characterBody.GetCollisionLayerValue(layer))
        {
            characterBody.SetCollisionLayerValue(layer, true);
        }

        if (inverted)
        {
            foreach(Lock locke in lockList)
            {
                locke.unlocked = true;
            }
            AttemptToClose();
            foreach(Lock locke in lockList)
            {
                locke.unlocked = false;
            }
        }
        
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
            //collisionShape.SetDeferred("one_way_collision", true);
        }
        return true;
    }

    public override bool AttemptToClose()
    {
       

        if (!base.AttemptToClose())
        {
            return false;
        }

        ToggleCollision();
            

            
            //collisionShape.SetDeferred("one_way_collision", false);
        
        return true;
    }

    void ToggleCollision()
    {
        
        if (inverted)
        {
            characterBody.SetCollisionLayerValue(layer, opened);
        sprite.Frame = opened ? 0 : 1;
        }
        else
        {
            characterBody.SetCollisionLayerValue(layer, !opened);
        sprite.Frame = opened ? 1 : 0;
        }

        
    }
    

}
