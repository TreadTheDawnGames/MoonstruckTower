using Godot;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;

public partial class EnemyPathfinder : Pathfinder
{
    public int movementDirection = 0;
    EnemyV2 logic;
    public bool takingDamage = false;
    public TileMapPathFind mapPather;
    public Vector2 lastLocation;
    bool pathfindCalledButNotOnFloor = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        base._Ready();
        mapPather = _pathFind2D;
        logic = GetNode<EnemyV2>("EnemyLogic");
        if (_pathFind2D == null)
        {
            _pathFind2D = (TileMapPathFind)GetTree().GetFirstNodeInGroup("PathfindMap");

        }

        //UnableToReachPoint += () => CreateAndGoToValidPath(_target.Position);
    }

    public override void PathfinderPhysicsProcess(double delta)
    {
        if((pathfindCalledButNotOnFloor||queueRepath)&&IsOnFloor())
        {
            CreateAndGoToValidPath(lastLocation);
            pathfindCalledButNotOnFloor= false;
            queueRepath = false;
        }
        if(!takingDamage)
        {
           base.PathfinderPhysicsProcess(delta);
        }
        else if (logic.damageTimer.TimeLeft > 0 || (takingDamage && !IsOnFloor()))
        {
            var velocityY = Velocity.Y;
            if (!IsOnFloor())
                velocityY = Velocity.Y + gravity * (float)delta;

            Velocity = new Vector2((float)Mathf.Lerp(Velocity.X, 0, 0.15), velocityY);
            MoveAndSlide();
        }


    }

    public override void PathfinderProcess(double delta)
    {
        if (IsOnFloor() && Input.IsActionJustPressed("Debug-Pathfind"))
        {
            CreateAndGoToValidPath((Vector2I)GetGlobalMousePosition());
        }
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

    protected override void CreateAndGoToPath(Vector2 where)
    {

        //_pathFind2D.AddVisualPoint(_pathFind2D.ConvertPointPositionToMapPosition(where), new Color(1f, 1f, 1, 1f), scale: 0.11f, pathfinder: this);

        var spaceState = GetWorld2D().DirectSpaceState;
        var query = PhysicsRayQueryParameters2D.Create(where, new Vector2(where[0], where[1] + 64), CollisionMask);
        var result = spaceState.IntersectRay(query);

        if (result.Count > 0)
        {

            Vector2I resultVector = (Vector2I)result["position"];
            Vector2I goTo = new Vector2I(resultVector.X, resultVector.Y - 16);

           // _pathFind2D.AddVisualPoint(_pathFind2D.ConvertPointPositionToMapPosition(new Vector2I(resultVector.X, resultVector.Y)), new Color(0f, 1f, 0.5f, 1f), scale: 0.60f, pathfinder: this);

            _pathFind2D.AddVisualPoint(_pathFind2D.ConvertPointPositionToMapPosition(goTo), new Color(1f, 1f, 0, 1f), scale: 1.2f, pathfinder: this, timer: 1);



            //_pathFind2D.AddVisualPoint(goTo, new Color(1f, 1f, 0, 1f), scale: 1.2f, pathfinder: this);
                DoPathFinding(goTo);
            
            return;
        }

    }

    public bool CreateAndGoToValidPath(Vector2 where)
    {
        if (!IsOnFloor()&&_target==null)
        {
            HaltPathing();
            
            pathfindCalledButNotOnFloor = true;
            lastLocation = where;
            return false;
        }


        var whereTile = _pathFind2D.ConvertPointPositionToMapPosition(where);
        var whereTileInfo = _pathFind2D.GetPointInfo(_pathFind2D.ConvertPointPositionToMapPosition(where));

        if (_pathFind2D.GetCellSourceId(0, whereTile) == -1 || (_pathFind2D.GetCellSourceId(0, _pathFind2D.ConvertPointPositionToMapPosition(where)) != -1 && whereTileInfo != null && whereTileInfo.IsDropthroughTile))
        {
            if((_pathFind2D.GetCellSourceId(0, _pathFind2D.ConvertPointPositionToMapPosition(where)) != -1 && whereTileInfo != null && whereTileInfo.IsDropthroughTile))
            
            {
                GD.Print("valid because dropthrough");
            }

            CreateAndGoToPath(where);
            return true;
        }
        else
        {
            var tileData = _pathFind2D.GetCellTileData(0, new Vector2I(whereTile.X, whereTile.Y));
            if (tileData != null)
            {
                if ((bool)tileData.GetCustomData("DropThroughTiles"))
                {
                    GD.Print("This is a dropthrough tile");
                    whereTile = new Vector2I(whereTile.X, whereTile.Y - 1);
                    //make end point the tile above selected tile
                    CreateAndGoToPath(where);//DoPathFinding(whereTile);
                    return true;
                }
            }
            else
            {
                GD.Print("Data is null");
            }
            ///if tile is on dropthrough layer
            ///return true
            ///


            GD.PushWarning("Attemped to go to inappropriate tile. ");
            _pathFind2D.AddVisualPoint(_pathFind2D.ConvertPointPositionToMapPosition(where), new Color(1f, 0.5f, 1, 1), scale: 1.5f, timer: 1);



            return false;
        }
    }

    public void HaltPathing()
    {
        StopPathfinding();
    }

    void TakeDamage(int amount, HitBox2D box)
    {
        logic.TakeDamage(amount, box);
    }

    public void Destroy()
    {
        QueueFree();
    }
}
