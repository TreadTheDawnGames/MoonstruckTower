using Godot;
using System;
using Godot.Collections;

[Icon("res://Assets/Locks and Doors/Icons/FallAwayFloorIcon.png")]
public partial class FallThroughDoor : Door
{
    Sprite2D sprite;
    CollisionShape2D collisionShape;
    StaticBody2D characterBody;
    [Export] int layer = 7;
    [Export] bool visible = true;
   

    public override void _Ready()
    {
        sprite = GetNode<Sprite2D>("Sprite2D");
        collisionShape = GetNode<CollisionShape2D>("StaticBody2D/CollisionShape2D");
        characterBody = GetNode<StaticBody2D>("StaticBody2D");
        if (!characterBody.GetCollisionLayerValue(layer))
        {
            characterBody.SetCollisionLayerValue(layer, true);
        }

        if (visible)
        {
            ToggleCollision();


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
            //collisionShape.SetDeferred("one_way_collision", true);
        }
        return true;
    }

    public override bool Close()
    {
        //GD.Print("Called Close");

        if (!base.Close())
        {
            return false;
        }

        if (!opened)
        {
            ToggleCollision();

        }
            

            
            //collisionShape.SetDeferred("one_way_collision", false);
        
        return true;
    }

    void ToggleCollision()
    {
        if (characterBody == null)
        {
            GD.PrintErr(Name + " has no character body");
            return;
        }

        if (visible)
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
