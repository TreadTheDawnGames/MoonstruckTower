using Godot;
using System;
using Godot.Collections;
using System.Collections.Immutable;

[Icon("res://Assets/Locks and Doors/Icons/FallAwayFloorIcon.png")]
public partial class FallThroughDoor : Door
{
    Sprite2D sprite;
    CollisionShape2D collisionShape;
    StaticBody2D characterBody;
    [Export] int layer = 7;
    [Export] bool visible = true;

    bool shouldPlaySound;
    bool lastLock;
    public override void _Ready()
    {
        sprite = GetNode<Sprite2D>("Sprite2D");
        collisionShape = GetNode<CollisionShape2D>("StaticBody2D/CollisionShape2D");
        characterBody = GetNode<StaticBody2D>("StaticBody2D");

        doorOpen = GD.Load<AudioStream>("res://Assets/Audio/SFX/Doors/DoorOn1.wav");
        doorClose = GD.Load<AudioStream>("res://Assets/Audio/SFX/Doors/DoorOff1.wav");

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

        shouldPlaySound = (lastLock != opened);
        if (characterBody == null)
        {
            GD.PrintErr(Name + " has no character body");
            return;
        }

        if (visible)
        {
            characterBody.SetCollisionLayerValue(layer, opened);
            sprite.Frame = opened ? 0 : 1;
            if (shouldPlaySound)
            {
                audioPlayer.PlaySound(opened ? doorOpen : doorClose);
                shouldPlaySound = false;
            }
        }
        else
        {
            characterBody.SetCollisionLayerValue(layer, !opened);
            sprite.Frame = opened ? 1 : 0;
            if (shouldPlaySound)
            {
                audioPlayer.PlaySound(opened ? doorClose:doorOpen);
                shouldPlaySound = false;
            }
        }

        lastLock = opened;
    }
    

}
