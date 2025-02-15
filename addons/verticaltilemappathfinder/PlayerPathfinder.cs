using Godot;
using System;
using System.Collections.Generic;
using System.Text;

public partial class PlayerPathfinder : CharacterBody2D
{
    [Export]
    public  float Speed = 100.0f;
    [Export]
    public  float JumpVelocity = -325.0f;
    [Export]
    public  float SmallJumpVelocity = -250.0f;
    [Export]
    public  float TinyJumpVelocity = -200.0f;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    private TileMapPathFind _pathFind2D;
    private Queue<PointInfo> _path = new Queue<PointInfo>();
    private PointInfo _target = null;
    private PointInfo _prevTarget = null;
    [Export]
    private float JumpDistanceHeightThreshold = 90.0f;

    public override void _Ready()
    {
        _pathFind2D = FindParent("Game").FindChild("TileMap") as TileMapPathFind;
    }

    private void GoToNextPointInPath()
    {
        // If there's no points in the path
        if (_path.Count <= 0)
        {
            _prevTarget = null; // Set previous target to null
            _target = null;     // Set target to null
            return;             // Return out of the method
        }
        _prevTarget = _target;  // Set the previous target to the current target
        _target = _path.Dequeue();  // Set the target node to the next target in the stack
        if (_prevTarget != null)
        {

            //GD.Print("Going to " + _target.Position);
            
            //_pathFind2D.AddVisualPoint(_pathFind2D.ConvertPointPositionToMapPosition(_target.Position), new Color(1, 0, 0, 1f), scale:0.75f);
        }
    }

    private void DoPathFinding()
    {
        var mousePos = GetGlobalMousePosition();

        var spaceState = GetWorld2D().DirectSpaceState;
        // use global coordinates, not local to node
        var query = PhysicsRayQueryParameters2D.Create(mousePos, new Vector2(mousePos[0], mousePos[1] + 1000));
        var result = spaceState.IntersectRay(query);

        if (result.Count > 0)
        {
            Vector2I resultVector = (Vector2I)result["position"];
            Vector2I goTo = new Vector2I(resultVector.X, resultVector.Y - 16);
            _path = _pathFind2D.GetPlaform2DPath(this.Position, goTo);
              GoToNextPointInPath();
        }

            

            
        
    }

    public override void _Process(double delta)
    {
        // If the character is on the ground, and the left mouse button was clicked
        if (IsOnFloor() && Input.IsActionJustPressed("Debug-Pathfind"))
        {


            DoPathFinding();
        }
    }
    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;
        Vector2 direction = Vector2.Zero;


        // Add the gravity.
        if (!IsOnFloor())
            velocity.Y += gravity * (float)delta;

        // if there is a target set
        if (_target != null)
        {
            // If the target is to the right of the current pusition
            if (_target.Position.X - 2 > Position.X)
            {
                direction.X = 1f;
            }
            // If the target is to the left of the current pusition
            else if (_target.Position.X + 2 < Position.X)
            {
                direction.X = -1f;
            }
            else
            {
                if (IsOnFloor())
                {
                    GoToNextPointInPath();
                    Jump(ref velocity);
                    Dropthrough(ref velocity);
                }
            }
        }

        if (direction != Vector2.Zero)
        {
            velocity.X = direction.X * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    private void Dropthrough(ref Vector2 velocity)
    {
        if (_prevTarget == null || _target == null || _target.IsPositionPoint)
        {
            return;
        }

        // If the current target is above the next target and the next target is a fall tile
        if (_prevTarget.Position.Y > _target.Position.Y && _target.IsFallTile)
        {
            return; // Return, no need to Drop
        }
        if (_prevTarget.Position.Y < _target.Position.Y || UseDropthrough())
        {
            //GD.Print("Used Dropthrough");
            GlobalPosition += new Vector2(0, 2);
            
            velocity.Y = 0;     //override jump velocity
        }
    }
        private bool UseDropthrough()
    {
        if (_prevTarget.IsDropthroughTile && _target.IsDropthroughFallTile&&
             _prevTarget.Position.Y < _target.Position.Y) // And the previous target is below the target tile
        {
            return true;    // Return true, perform the fall
        }
        return false;       // return false, don't perform the jump
    }
    private bool JumpRightEdgeToLeftEdge()
    {
        // If the previous tile was a right edge, and the target tile is a left edge
        if (_prevTarget.IsRightEdge && _target.IsLeftEdge
        && _prevTarget.Position.Y <= _target.Position.Y // And the previous target is below the target tile
        && _prevTarget.Position.X < _target.Position.X  // And the previous target is to the left of the target
        ) 
        {
            
            //GD.Print("Jump because Right to Left");

            return true;    // Return true, perform the jump
        }
        return false;       // return false, don't perform the jump
    }



    private bool JumpLeftEdgeToRightEdge()
    {
        // If the previous tile was a left edge, and the target tile is a right edge
        if (_prevTarget.IsLeftEdge && _target.IsRightEdge
        && _prevTarget.Position.Y <= _target.Position.Y // And the previous target is below the target tile
        && _prevTarget.Position.X > _target.Position.X  // And the previous target is to the right of the target
        ) 
        {
            //GD.Print("Jump because Left To Right");

            return true;    // Return true, perform the jump
        }
        return false;       // return false, don't perform the jump
    }
    private bool JumpToDropthrough()
    {

        // If the previous tile was a DropdownFall tile, and the target tile is a Dropthrough tile
        if (_prevTarget.IsDropthroughFallTile && _target.IsDropthroughTile
        && _prevTarget.Position.Y >= _target.Position.Y // And the previous target is above the target tile
        && _prevTarget.Position.X == _target.Position.X) // And the previous target is to the right of the target
        {
        //GD.Print("Jump because Dropthrough");
            return true;    // Return true, perform the jump
        }
        return false;       // return false, don't perform the jump
    }

    bool JumpBecauseTooLow()
    {
        if(_prevTarget.Position.Y -8 > _target.Position.Y)
        {
            //GD.Print("Jump because Too Low");
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
        if (_prevTarget.Position.Y < _target.Position.Y
        && _prevTarget.Position.DistanceTo(_target.Position) < JumpDistanceHeightThreshold)
        {
            return;
        }

        // If the current target is above the next target and the next target is a fall tile
        if (_prevTarget.Position.Y < _target.Position.Y && _target.IsFallTile)
        {
            return; // Return, no need to jump
        }


        if ((JumpToDropthrough()||JumpBecauseTooLow() || JumpRightEdgeToLeftEdge() || JumpLeftEdgeToRightEdge())&&!UseDropthrough())
        {
            int heightDistance = _pathFind2D.LocalToMap(_target.Position).Y - _pathFind2D.LocalToMap(_prevTarget.Position).Y;
            if (Mathf.Abs(heightDistance) <= 1.5)
            {
                //GD.Print("Tiny Jump");
                velocity.Y = TinyJumpVelocity;
            }
            else if (Mathf.Abs(heightDistance) == 2.75)
            {
                //GD.Print("Medium Jump");
                velocity.Y = SmallJumpVelocity;
            }
            else
            {
                //GD.Print("High Jump");
                velocity.Y = JumpVelocity;
            }
        }
    }

}
