using Godot;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Emit;

[Icon("res://Assets/Locks and Doors/Icons/WallDoorIcon.png")]
public partial class WallDoor : Door
{
    AnimationPlayer animator;

    bool animationOpen = false;
    [Export] bool startOpen = false;
    Area2D checkIfBodyOnTop;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        animator = GetNode<AnimationPlayer>("AnimationPlayer");
        checkIfBodyOnTop = GetNode<Area2D>("CheckIfBodyOnTop");

        //base._Ready();
        if (startOpen)
        {
            foreach (Lock locke in lockList)
            {
                locke.unlocked = true;
            }
            AttemptToOpen();
            foreach (Lock locke in lockList)
            {
                locke.unlocked = false;
            }
        }
        animator.AnimationFinished += AnimationFinished;
    }

    void AnimationFinished(StringName animation)
    {
        if(checkIfBodyOnTop.HasOverlappingBodies())
        {
            Player link = null;
            bool exit = false ;
                GD.Print(checkIfBodyOnTop.GetOverlappingBodies().Count);
            
            if (checkIfBodyOnTop.GetOverlappingBodies().Count <= 1)
            {
                exit = true;               
            }

            if (exit)
            {
                GD.Print("No need");
                return;
            }
            foreach (var body in checkIfBodyOnTop.GetOverlappingBodies().OfType<Player>())
            {
                link = body;
            }

            

            foreach (var body in checkIfBodyOnTop.GetOverlappingBodies())
            {
                if(body == this)
                {
                    GD.Print("Myself");
                    continue;
                }

                int direction = Mathf.Sign(link.GlobalPosition.X - GlobalPosition.X);

                link.GlobalPosition += new Vector2(direction, 8);

                GD.Print(body.GetType().ToString() + " " + body.Name) ;
            }
/*
            foreach(PhysicsBody2D body in checkIfBodyOnTop.GetOverlappingBodies().OfType<PhysicsBody2D>())
            {
                
                    GD.Print("Hello World!");
                
            }*/
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
        

        if (startOpen)
        {


            if (opened)
            {
                if (animationOpen)
                {
                    animator.PlayBackwards("Open");
                    animationOpen = false;
                }
            }
            else
            {
                if (!animationOpen)
                {
                    animator.Play("Open");
                    animationOpen = true;
                }
            }

        }
        else
        {
            if (opened)
            {
                if (!animationOpen)
                {
                    animator.Play("Open");
                    animationOpen = true;
                }
            }
            else
            {
                if (animationOpen)
                {
                    animator.PlayBackwards("Open");
                    animationOpen = false;
                }
            }

        }


    }


}
