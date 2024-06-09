using Godot;
using System;
using System.Diagnostics;

public partial class EnemyPathfinder : Pathfinder
{
    [Signal]
    public delegate void UnreachablePointEventHandler();
    public int movementDirection = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        base._Ready();
        if (_pathFind2D == null)
        {
            _pathFind2D = (TileMapPathFind)GetTree().GetFirstNodeInGroup("PathfindMap");

        }
    }

    public override void _Process(double delta)
    {
        if (Velocity.X > 0)
        {
            movementDirection = 1;

        }
        else if (Velocity.X < 0)
        {
            movementDirection = -1;

        }
        else { movementDirection = 0; }
    }

    public override void CreateAndGoToPath(Vector2 where)
    {
        var aboveWhere = _pathFind2D.ConvertPointPositionToMapPosition(where);
        aboveWhere.Y += 1;
        if(_pathFind2D.GetCellSourceId(0, aboveWhere)==-1&&IsOnFloor())
        {

            var spaceState = GetWorld2D().DirectSpaceState;
            // use global coordinates, not local to node
            var query = PhysicsRayQueryParameters2D.Create(where, new Vector2(where[0], where[1] + 1000));
            var result = spaceState.IntersectRay(query);
            if (result.Count > 0)
            {
                Vector2I resultVector = (Vector2I)result["position"];
                Vector2I goTo = new Vector2I(resultVector.X, resultVector.Y - 16);

                
                _pathFind2D.AddVisualPoint(_pathFind2D.ConvertPointPositionToMapPosition(new Vector2(goTo.X, goTo.Y)), new Color(1f, 1f, 0, 1f), scale: 1.2f, pathfinder: this);
                
                DoPathFinding(goTo);
                return;
            }
        }
        EmitSignal(SignalName.UnreachablePoint);
    }
}
