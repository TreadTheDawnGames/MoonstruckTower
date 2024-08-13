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
    Area2D checkRightWall;
    Area2D checkLeftWall;

    [Export] bool shortWall = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        animator = GetNode<AnimationPlayer>("AnimationPlayer");
        checkIfBodyOnTop = GetNode<Area2D>("CheckIfBodyOnTop");
        checkRightWall = GetNode<Area2D>("CheckRightWall");
        checkLeftWall = GetNode<Area2D>("CheckLeftWall");

        //base._Ready();
        if (startOpen)
        {
            foreach (Lock locke in lockList)
            {
                locke.unlocked = true;
            }
            foreach (Lock locke in lockList)
            {
                locke.unlocked = false;
            }
        }
            AttemptToOpen();
        animator.AnimationFinished += AnimationFinished;
    }

    void AnimationFinished(StringName animation)
    {
        if(checkIfBodyOnTop.HasOverlappingBodies())
        {
            Player link = null;
            bool exit = false ;
                //GD.Print(checkIfBodyOnTop.GetOverlappingBodies().Count);
            
            if (checkIfBodyOnTop.GetOverlappingBodies().Count <= 1)
            {
                exit = true;               
            }

            if (exit)
            {
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
                    //GD.Print("Myself");
                    continue;
                }
                int direction;
                if (checkLeftWall.GetOverlappingBodies().OfType<TileMap>().Count()>0)
                {
                    direction = 1;
                    //GD.Print("on left wall");
                }
                else if (checkRightWall.GetOverlappingBodies().OfType<TileMap>().Count()>0)
                {
                    direction = -1;
                    //GD.Print("on right wall");
                }
                else
                {
                    direction = Mathf.Sign(link.GlobalPosition.X - GlobalPosition.X);
                    if (direction == 0)
                        direction = 1;
                }

                int y = 10;

                if (shortWall)
                {
                    y = 0;
                }

                link.GlobalPosition += new Vector2(direction*2, y);


                //GD.Print(body.GetType().ToString() + " " + body.Name) ;
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
                    audioPlayer.PlaySound(doorClose);

                    animator.PlayBackwards("Open");
                    animationOpen = false;
                }
            }
            else
            {
                if (!animationOpen)
                {
                    audioPlayer.PlaySound(doorOpen);

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
                    audioPlayer.PlaySound(doorOpen);

                    animator?.Play("Open");
                    animationOpen = true;
                }
            }
            else
            {
                if (animationOpen)
                {
                    audioPlayer.PlaySound(doorClose);

                    animator?.PlayBackwards("Open");
                    animationOpen = false;
                }
            }

        }



    }


}
