using Godot;
using System;
using System.Diagnostics;

public partial class EnemyPathfinder : Pathfinder
{
    [Signal]
    public delegate void UnreachablePointEventHandler();
    public int movementDirection = 0;
    EnemyV2 logic;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        base._Ready();
        logic = GetNode<EnemyV2>("EnemyLogic");
        if (_pathFind2D == null)
        {
            _pathFind2D = (TileMapPathFind)GetTree().GetFirstNodeInGroup("PathfindMap");

        }
    }

    public override void PathfinderPhysicsProcess(double delta)
    {
       base.PathfinderPhysicsProcess(delta);
    }

    public override void PathfinderProcess(double delta)
    {

        base.PathfinderProcess(delta);
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
        _pathFind2D.AddVisualPoint(_pathFind2D.ConvertPointPositionToMapPosition(where), new Color(1f, 1f, 1, 1f), scale: 0.11f, pathfinder: this);


        var spaceState = GetWorld2D().DirectSpaceState;
        var query = PhysicsRayQueryParameters2D.Create(where, new Vector2(where[0], where[1] + 1000), CollisionMask);
        var result = spaceState.IntersectRay(query);

        if (result.Count > 0)
        {
            Vector2I resultVector = (Vector2I)result["position"];
            Vector2I goTo = new Vector2I(resultVector.X, resultVector.Y - 16);

            _pathFind2D.AddVisualPoint(_pathFind2D.ConvertPointPositionToMapPosition(new Vector2I(resultVector.X, resultVector.Y)), new Color(0f, 1f, 0.5f, 1f), scale: 0.60f, pathfinder: this);

            _pathFind2D.AddVisualPoint(_pathFind2D.ConvertPointPositionToMapPosition(goTo), new Color(1f, 1f, 0, 1f), scale: 1.2f, pathfinder: this);



            //_pathFind2D.AddVisualPoint(goTo, new Color(1f, 1f, 0, 1f), scale: 1.2f, pathfinder: this);

            DoPathFinding(goTo);
            return;
        }

    }

    public void HaltPathing()
    {
        StopPathfinding();
    }

    void TakeDamage(int amount)
    {
        logic.TakeDamage(amount);
    }

    public void Destroy()
    {
        QueueFree();
    }
}
