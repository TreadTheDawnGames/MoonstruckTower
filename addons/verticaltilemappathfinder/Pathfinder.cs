using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
public partial class Pathfinder : CharacterBody2D
{
    [Export]
    public float Speed = 100.0f;
    [Export]
    public float PixelJumpHeight = 64;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    protected TileMapPathFind _pathFind2D;
    protected Queue<PointInfo> _path = new Queue<PointInfo>();
    protected PointInfo _target = null;
    protected PointInfo _prevTarget = null;
    [Export]
    private float JumpDistanceHeightThreshold = 48f;

    [Signal]
    public delegate void PathfindEndEventHandler();
    [Signal]
    public delegate void ReachedPointEventHandler();
    [Signal]
    public delegate void UnableToReachPointEventHandler();
    protected bool movingRight;

    protected bool queueRepath = false;

    public override void _Ready()
    {
        //_pathFind2D = FindParent("Game").FindChild("TileMap") as TileMapPathFind;
        _pathFind2D = (TileMapPathFind)GetTree().GetFirstNodeInGroup("PathfindMap");

    }

    protected void GoToNextPointInPath()
    {

        var globalLocalMapPosision = _pathFind2D.ConvertPositionToLocalMapPosition(GlobalPosition);
        if(_target!=null)
        {
            if ((_target.Position.X + 2 > globalLocalMapPosision.X && _target.Position.X - 2 < globalLocalMapPosision.X) && (_target.Position.Y + 2 > globalLocalMapPosision.Y && _target.Position.Y - 2 < globalLocalMapPosision.Y))
            {
                //GD.Print("Reached point " + _target.Position);
                EmitSignal(SignalName.ReachedPoint);
            }
            if ((_target.Position.X + 2 > globalLocalMapPosision.X && _target.Position.X - 2 < globalLocalMapPosision.X) && !(_target.Position.Y + 2 > globalLocalMapPosision.Y && _target.Position.Y - 2 < globalLocalMapPosision.Y))
            {
                //GD.Print("Under or Over point " + _target.Position);
                EmitSignal(SignalName.UnableToReachPoint);
               
                //when under or over target path queue repath.
                
                //EmitSignal(SignalName.UnableToReachPoint);
                //_path = new();
            }
        }

        

        // If there's no points in the path
        if (_path.Count <= 0)
        {
            ////GD.Print("\n");
            EmitSignal(SignalName.PathfindEnd);
            _prevTarget = null; // Set previous target to null
            _target = null;     // Set target to null
            return;             // Return out of the method
        }
        
        _prevTarget = _target;  // Set the previous target to the current target
        _target = _path.Dequeue();  // Set the target node to the next target in the stack
                                    // //GD.Print("Going to " + _target.Position);

            
        if (_pathFind2D.ConvertPositionToLocalMapPosition(GlobalPosition) == _target.Position)
            EmitSignal(SignalName.ReachedPoint);

        if (_prevTarget != null)
        {
            ////GD.Print("Going to " + _target.Position);
            _pathFind2D.AddVisualPoint(_pathFind2D.ConvertPointPositionToMapPosition(_target.Position), new Color(1, 0, 0, 1f), scale: 0.75f, this);
        }
    }

    protected virtual void DoPathFinding(Vector2I goTo)
    {
        //_pathFind2D.AddVisualPoint(goTo);

        _path = _pathFind2D.GetPlaform2DPath(this.GlobalPosition, goTo);

        GoToNextPointInPath();
    }
    protected void StopPathfinding()
    {
        
            _path.Clear();
//        _path = new();
        GoToNextPointInPath();
    }

    protected virtual void CreateAndGoToPath(Vector2 where) { GD.PrintErr("Pathfinder is not inteded to be used directly. Please write goTo parsing logic in a script that inherits it."); }

    public virtual void PathfinderProcess(double delta)
    {
        // If the character is on the ground, and the left mouse button was clicked
        if (IsOnFloor() && Input.IsActionJustPressed("Debug-Pathfind"))
        {
            DoPathFinding((Vector2I)GetGlobalMousePosition());
        }
    }
    public virtual void PathfinderPhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;
        // Add the gravity.
        if (!IsOnFloor())
            velocity.Y += gravity * (float)delta;
        var moveDirection = Vector2.Zero;
        // if there is a target set
        if (_target != null)
        {
            // If the target is to the right of the current pusition
            if (_target.Position.X - 2 > GlobalPosition.X)
            {
                movingRight = true;
                moveDirection.X = 1f;
            }
            // If the target is to the left of the current pusition
            else if (_target.Position.X + 2 < GlobalPosition.X)
            {
                movingRight = false;
                moveDirection.X = -1f;
            }
            else
            {
                //if enemy misses target it gets stuck because it won't repath until it reaches the end of the line, and it can't do that because it won't go to the next point
                //because it's not in range.

                //Result is enemy freezed below/above target. Fix by adding freeze timer?
                if (IsOnFloor() && _pathFind2D.ConvertPositionToLocalMapPosition(GlobalPosition).DistanceTo(_target.Position) < 16)
                {
                    
                    

                    GoToNextPointInPath();
                    Jump(ref velocity);
                    Dropthrough(ref velocity);
                }
            }
        }
        if (moveDirection != Vector2.Zero)
        {
            velocity.X = moveDirection.X * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
        }
        Velocity = velocity;
        MoveAndSlide();
        moveDirection = Vector2.Zero;
    }
    private void Dropthrough(ref Vector2 velocity)
    {
        if (_prevTarget == null || _target == null || _target.IsPositionPoint)
        {
            return;
        }

        // If the current target is above the next target and the next target is a fall tile
        if (_pathFind2D.ConvertPositionToLocalMapPosition(GlobalPosition).Y > _target.Position.Y && _target.IsFallTile)
        {
            return; // Return, no need to Drop
        }
        if (UseDropthrough()/*||UseDropthrouthBecauseLower()*/ )
        {
            GlobalPosition += new Vector2(0, 2);
            //velocity.Y = 0;     //override jump velocity
        }
    }
    bool UseDropthrouthBecauseLower()
    {
        if (
             _pathFind2D.ConvertPositionToLocalMapPosition(GlobalPosition).Y < _target.Position.Y) // And the previous target is below the target tile
        {
            //_pathFind2D.AddVisualPoint(_pathFind2D.ConvertPointPositionToMapPosition(_prevTarget.Position), new Color(0.5f, 0.5f, 0.5f), scale: 2);
            //_pathFind2D.AddVisualPoint(_pathFind2D.ConvertPointPositionToMapPosition(_target.Position), new Color(0.5f, 0.5f, 0.5f), scale: 2);

           // //GD.Print("Used Dropthrough becuase lower");
            return true;    // Return true, perform the fall
        }
        return false;
    }
    private bool UseDropthrough()
    {
        if (_prevTarget.IsDropthroughTile /*&& _target.IsDropthroughFallTile*/
         && _prevTarget.Position.Y < _target.Position.Y // And the previous target is above the target tile
         && _target.Position.X == _prevTarget.Position.X
         && IsOnFloor())
        {
         //   //GD.Print("Used Dropthrough because dropthrough");
            return true;    // Return true, perform the fall
        }
        return false;       // return false, don't perform the jump
    }
    private bool JumpRightEdgeToLeftEdge()
    {
        // If the previous tile was a right edge, and the target tile is a left edge
        if (_prevTarget.IsRightEdge && _target.IsLeftEdge
        && _pathFind2D.ConvertPositionToLocalMapPosition(GlobalPosition).Y >= _target.Position.Y // And the previous target is below the target tile
        && _pathFind2D.ConvertPositionToLocalMapPosition(GlobalPosition).X < _target.Position.X  // And the previous target is to the left of the target
        )
        {

            // //GD.Print("Jump because Right to Left");
            return true;    // Return true, perform the jump
        }
        return false;       // return false, don't perform the jump
    }
    private bool JumpLeftEdgeToRightEdge()
    {
        // If the previous tile was a left edge, and the target tile is a right edge
        if (_prevTarget.IsLeftEdge && _target.IsRightEdge
        && _pathFind2D.ConvertPositionToLocalMapPosition(GlobalPosition).Y >= _target.Position.Y // And the previous target is below the target tile
        && _pathFind2D.ConvertPositionToLocalMapPosition(GlobalPosition).X > _target.Position.X  // And the previous target is to the right of the target
        )
        {
            // //GD.Print("Jump because Left To Right");
            return true;    // Return true, perform the jump
        }
        return false;       // return false, don't perform the jump
    }
    private bool JumpToDropthrough()
    {

        // If the previous tile was a DropdownFall tile, and the target tile is a Dropthrough tile
        if (_prevTarget.IsDropthroughFallTile && _target.IsDropthroughTile
        && _pathFind2D.ConvertPositionToLocalMapPosition(GlobalPosition).Y >= _target.Position.Y // And the previous target is above the target tile
        && _pathFind2D.ConvertPositionToLocalMapPosition(GlobalPosition).X == _target.Position.X) // And the previous target is to the right of the target
        {
            // //GD.Print("Jump because Dropthrough");
            return true;    // Return true, perform the jump
        }
        return false;       // return false, don't perform the jump
    }

    bool JumpBecauseTooLow()
    {
        if (_pathFind2D.ConvertPositionToLocalMapPosition(GlobalPosition).Y - 8 > _target.Position.Y)
        {
            //   //GD.Print("Jump because Too Low");
            return true;
        }
        return false;
    }
    private void Jump(ref Vector2 velocity)
    {
        if (_prevTarget == null || _target == null || _target.IsPositionPoint)
        {
            return;
        }

        // If the previous target is above the target, and the distance is less than the jump height threshold
        if (_pathFind2D.ConvertPositionToLocalMapPosition(GlobalPosition).Y < _target.Position.Y
        && _pathFind2D.ConvertPositionToLocalMapPosition(GlobalPosition).DistanceTo(_target.Position) < JumpDistanceHeightThreshold
        )
        {
            return;
        }

        // If the current target is above the next target and the next target is a fall tile
        if (_pathFind2D.ConvertPositionToLocalMapPosition(GlobalPosition).Y < _target.Position.Y && _target.IsFallTile)
        {
            return; // Return, no need to jump
        }


        if ((JumpToDropthrough() || JumpBecauseTooLow() || JumpRightEdgeToLeftEdge() || JumpLeftEdgeToRightEdge()) && !UseDropthrough())
        {
            float jumpDistance = _target.Position.DistanceTo(_pathFind2D.ConvertPositionToLocalMapPosition(GlobalPosition));

            var jumpPower = Mathf.Clamp(PixelJumpHeight * (Mathf.Abs(jumpDistance) / PixelJumpHeight),0,PixelJumpHeight);

            float jumpInPixels = -Mathf.Sqrt(2 * gravity * jumpPower);
           
            velocity.Y = jumpInPixels; // JumpVelocity* (Mathf.Abs(heightDistance));
        }
    }
}