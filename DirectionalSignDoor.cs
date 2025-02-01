using Godot;
using System;
using System.Collections.Generic;

public partial class DirectionalSignDoor : Door, IDoor
{
    enum SignDirection { Up, Down, Side, DiagUp, DiagDown }
    [Export] SignDirection direction;

    [Export] bool faceRight = false;

    AnimationPlayer animator;
    Sprite2D sprite;
    string animation;
    bool animationOpen = false;

    public override void _EnterTree()
    {
        base._EnterTree();
        animator = GetNode<AnimationPlayer>("AnimationPlayer");
        sprite = GetNode<Sprite2D>("Sprite2D");

        sprite.FlipH = faceRight;


        switch (direction)
        {
            case SignDirection.Up:
                animation = "Up";
                break;

            case SignDirection.Down:
                animation = "Down";
                break;

            case SignDirection.Side:
                animation = "Side";
                break;

            case SignDirection.DiagUp:
                animation = "DiagUp";
                break;

            case SignDirection.DiagDown:
                animation = "DiagDown";
                break;

        }
    }

    public override bool AttemptToOpen()
    {


        base.AttemptToOpen();



        ToggleCollision();

        return true;
    }

    public override bool Close()
    {
        if (!base.Close())
        {
            return false;
        }

        /*if(!opened)
        {
            ToggleCollision();
        }*/

        return true;
    }


    void ToggleCollision()
    {


       


            if (!opened)
            {
                if (animationOpen)
                {
                    animator.PlayBackwards(animation);
                    animationOpen = false;
                }
            }
            else
            {
                if (!animationOpen)
                {
                    animator.Play(animation);
                    animationOpen = true;
                }
            }

       
            

        


    }
}
