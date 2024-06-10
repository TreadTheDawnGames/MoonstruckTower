using Godot;
using System;
using Godot.Collections;
using System.Net.Sockets;
using System.Linq;
using System.Transactions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Net;

public class PointInfo
{
	public bool IsFallTile;
	public bool IsDropthroughFallTile;
	public bool IsLeftEdge;
	public bool IsRightEdge;
	public bool IsLeftWall;
	public bool IsRightWall;
	public bool IsPositionPoint;
	public bool IsDropthroughTile;
	public bool IsLadderPoint;
	public long PointID;
	public Vector2 Position;
	public PointInfo() { }
	public PointInfo(long pointID,Vector2 position)
	{
		PointID = pointID;
		Position = position;
		
	}

	public void PrintInfo()
	{
		GD.Print(	 "PointID = " + PointID				 
				 + "\nPosition = " + Position				 
				 + "\nIsFallTile = "+  IsFallTile			 
				 + "\nIsDropthroughFallTile = " + IsDropthroughFallTile 
				 + "\nIsLeftEdge = " + IsLeftEdge			 
				 + "\nIsRightEdge = " + IsRightEdge			 
				 + "\nIsLeftWall = " + IsLeftWall			 
				 + "\nIsRightWall = " + IsRightWall			 
				 + "\nIsPositionPoint = " + IsPositionPoint		 
				 + "\nIsDropthroughTile = " + IsDropthroughTile	 
				 + "\nIsLadderPoint = " + IsLadderPoint	+ "\n"	 
				 );		 

    }

}

public partial class TileMapPathFind : TileMap
{
	[Export]
	public bool ShowDebugGraph = true;
	[Export]
	public double JumpDistance = 5;
	[Export]
	public double JumpHeight = 4.5;

	private const int COLLISION_LAYER = 0;
	private const int CELL_IS_EMPTY = -1;
	private const int MAX_TILE_FALL_SCAN_DEPTH = 500;

	private AStar2D _astarGraph = new AStar2D();
	private Array<Vector2I> _usedTiles;
	private PackedScene _graphPoint;

	private System.Collections.Generic.List<PointInfo> _pointInfoList = new System.Collections.Generic.List<PointInfo>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_graphPoint = ResourceLoader.Load<PackedScene>("res://addons/VerticalTileMapPathFinder/GraphPoint.tscn");

		_usedTiles = GetUsedCells(COLLISION_LAYER);

		BuildGraph();

		

    }

	private void BuildGraph()
	{
		foreach (var tile in _usedTiles)
		{
			AddLeftEdgePoint(tile);
			AddRightEdgePoint(tile);
			AddLeftWallPoint(tile);
			AddRightWallPoint(tile);
            AddFallPoint(tile);
			AddDropthroughPoints(tile);
			//AddRightDropThroughStairPoints(tile);
        }

		
	}

	private void AddDropthroughPoints(Vector2I tile)
	{
			AddDropthroughPoint(tile);
			AddDropthroughFallPoint(tile);

		
	}
	

    #region Helper Methods
	public Vector2I ConvertPointPositionToMapPosition(Vector2 tile)
	{
		return LocalToMap(tile);
	}
	
    private PointInfo GetPointInfoAtPosition(Vector2 position)
	{
		var newInfoPoint = new PointInfo(-10000, position);     // Create a new PointInfo with the position
		newInfoPoint.IsPositionPoint = true;                    // Mark it as a position point
		var tile = LocalToMap(position);                        // Get the tile position		
		//GD.Print(tile);

		// If a tile is found below
		if (GetCellSourceId(COLLISION_LAYER, new Vector2I(tile.X, tile.Y + 1)) != CELL_IS_EMPTY)
		{
			// If a tile exist to the left
			if (GetCellSourceId(COLLISION_LAYER, new Vector2I(tile.X - 1, tile.Y)) != CELL_IS_EMPTY)
			{
				newInfoPoint.IsLeftWall = true;   // Flag that it's a left wall
			}
			// If a tile exist to the right
			if (GetCellSourceId(COLLISION_LAYER, new Vector2I(tile.X + 1, tile.Y)) != CELL_IS_EMPTY)
			{
				newInfoPoint.IsRightWall = true;  // Flag that it's a right wall
			}
			// If a tile doesn't exist one tile below to the left
			if (GetCellSourceId(COLLISION_LAYER, new Vector2I(tile.X - 1, tile.Y + 1)) != CELL_IS_EMPTY)
			{
				newInfoPoint.IsLeftEdge = true;  // Flag that it's a left edge
			}
			// If a tile doesn't exist one tile below to the right
			if (GetCellSourceId(COLLISION_LAYER, new Vector2I(tile.X + 1, tile.Y + 1)) != CELL_IS_EMPTY)
			{
				newInfoPoint.IsRightEdge = true;  // Flag that it's a right edge
			}
		}

		return newInfoPoint;
	}

	private System.Collections.Generic.Stack<PointInfo> ReversePathStack(System.Collections.Generic.Stack<PointInfo> pathStack)
	{
		System.Collections.Generic.Stack<PointInfo> pathStackReversed = new();


		while (pathStack.Count != 0)
		{
			pathStackReversed.Push(pathStack.Pop());
		}
		return pathStackReversed;
	}

	private Vector2 CheckForHorizontalPoints(Vector2 position, string debugName = "")
	{
		if (MapToLocal(LocalToMap(position)).Y == MapToLocal(LocalToMap(_astarGraph.GetClosestPositionInSegment(position))).Y)
		{
			Vector2 returnPos = position;
			var leftPoint = new Vector2(position.X - 10000, position.Y);
			var rightPoint = new Vector2(position.X + 10000, position.Y);

			foreach (var point in _pointInfoList)
			{                                    //position is to the left
				if (MapToLocal(LocalToMap(point.Position)).Y == MapToLocal(LocalToMap(position)).Y && point.Position.X < position.X)
				{
					if (point.Position.DistanceTo(position) < leftPoint.DistanceTo(position))
					{
						leftPoint = point.Position;
						//GD.Print(debugName + "Distance L " + leftPoint.DistanceTo(position));
					}
				}
			                                   //position is to the Right
				if (MapToLocal(LocalToMap(point.Position)).Y == MapToLocal(LocalToMap(position)).Y && point.Position.X > position.X)
				{
					if (point.Position.DistanceTo(position) < rightPoint.DistanceTo(position))
					{
						rightPoint = point.Position;
					//	GD.Print(debugName + "Distance R " + rightPoint.DistanceTo(position));
					}
				}
				//check left and right for points
				//if left is closer than right use that else vice versa
			}
			//GD.Print(debugName +"left point: " + LocalToMap(rightPoint));

			if (Mathf.Abs(leftPoint.DistanceTo(position)) < Mathf.Abs(rightPoint.DistanceTo(position)))
			{
                //GD.Print(debugName + "Left returned");

                returnPos = leftPoint;
			}
			else if (Mathf.Abs(leftPoint.DistanceTo(position)) > Mathf.Abs(rightPoint.DistanceTo(position)))
			{
               // GD.Print(debugName + "Right returned");

                returnPos = rightPoint;
			}
			//GD.Print("Right: " + Mathf.Abs(rightPoint.DistanceTo(position)) + "\n Left: " + Mathf.Abs(leftPoint.DistanceTo(position)));

			return returnPos;
		}
		else return position;
    }

	public System.Collections.Generic.Stack<PointInfo> GetPlaform2DPath(Vector2 startPos, Vector2 endPos)
	{
		System.Collections.Generic.Stack<PointInfo> pathStack = new System.Collections.Generic.Stack<PointInfo>();

		
		
		//if endpoint == astar.getclosestpointonpath
        //var endPoint = GetPointInfo(LocalToMap(endPos));
        //if (endPoint == null) endPoint = GetPointInfoAtPosition(endPos);
        //var idPath = _astarGraph.GetIdPath(_astarGraph.GetClosestPoint(startPos), endPoint.Position);
        
		
        // Find the path between the start and end position
        var idPath = _astarGraph.GetIdPath(_astarGraph.GetClosestPoint(CheckForHorizontalPoints(startPos, "Start ")), _astarGraph.GetClosestPoint(CheckForHorizontalPoints(endPos, "End ")));

		if (idPath.Count() <= 0) { return pathStack; }      // If the the path has reached its goal, return the empty path stack

		var startPoint = GetPointInfoAtPosition(startPos);  // Create the point for the start position		
		var endPoint = GetPointInfo(LocalToMap(endPos));
		if(endPoint == null) endPoint = GetPointInfoAtPosition(endPos);      // Create the point for the end position		
		var numPointsInPath = idPath.Count();               // Get number of points in the astar path
		

		// loop through all the points in the path
		for (int i = 0; i < numPointsInPath; ++i)
		{
			var currPoint = GetInfoPointByPointId(idPath[i]);   // Get the current point in the idPath		

			// If there's only one point in the path
			if (numPointsInPath == 1)
			{
				continue;   // Skip the point in the aStar path, the end point will be added as the only path point at the end.
			}
			// If it's the first point in the astar path
			if (i == 0 && numPointsInPath >= 2)
			{
				// Get the next second path point in the astar path
				var secondPathPoint = GetInfoPointByPointId(idPath[i + 1]);

				// If the start point is closer to the second path point than the current point
				if (startPoint.Position.DistanceTo(secondPathPoint.Position) < currPoint.Position.DistanceTo(secondPathPoint.Position))
				{
					pathStack.Push(startPoint); // Add the start point to the path
					continue;                   // Skip adding the current point and go to the next point in the path
				}
			}
			// If it's the last point in the path 
			else if (i == numPointsInPath - 1 && numPointsInPath >= 2)
			{
				// Get the penultimate point in the astar path list
				var penultimatePoint = GetInfoPointByPointId(idPath[i - 1]);

				if(endPoint == currPoint)
				{
					//GD.Print("LAST POINT == CURRPOINT");
					continue;
				}

				// If the endPoint is closer than the last point in the astar path
				if (endPoint.Position.DistanceTo(penultimatePoint.Position) < currPoint.Position.DistanceTo(penultimatePoint.Position))
				{
					continue;                   // Skip adding the last point to the path stack
				}
				// If the last point is closer
				else
				{
					pathStack.Push(currPoint);  // Add the current point to the path stack
					break;                      // Break out of the for loop
				}
			}

			pathStack.Push(currPoint);      // Add the current point			
		}
		pathStack.Push(endPoint);           // Add the end point to the path
		return ReversePathStack(pathStack); // Return the pathstack reversed		
	}

	private PointInfo GetInfoPointByPointId(long pointId)
	{
		// Find and return the first point in the _pointInfoList with the given pointId
		return _pointInfoList.Where(p => p.PointID == pointId).FirstOrDefault();
	}

	private void DrawDebugLine(Vector2 to, Vector2 from, Color color)
	{
		if (ShowDebugGraph)
		{
			
			DrawLine(from, to, color);
		}
	}


	private PointInfo GetPointInfo(Vector2I tile)
	{
		foreach (var pointInfo in _pointInfoList)
		{
			if (pointInfo.Position == MapToLocal(tile))
			{
				return pointInfo;
			}
		}
		return null;
	}
    #endregion

    public override void _Draw()
	{
		//if (ShowDebugGraph)
		{
			ConnectPoints();
		}
	}

	#region Connect Graph Points
	private void ConnectPoints()
	{
		foreach (var p1 in _pointInfoList)
		{
			ConnectHorizontalPoints(p1);
			ConnectJumpPoints(p1);
			ConnectFallPoint(p1);
		}
	}

	


    private void ConnectFallPoint(PointInfo p1, bool ignoreDraw = false)
	{

		if (p1.IsLeftEdge || p1.IsRightEdge)
		{
			var tilePos = LocalToMap(p1.Position);

			tilePos.Y += 1;

			if (FindFallPoints(tilePos) == null) return;
			foreach(Vector2I? fallPoint in FindFallPoints(tilePos))
			{
				CreateConnectionForFallPoint(p1, fallPoint, ignoreDraw);
			}
		}
	}

	private void CreateConnectionForFallPoint(PointInfo p1, Vector2I? fallPoint, bool ignoreDraw = false)
	{
        if (fallPoint != null)
        {
            var pointInfo = GetPointInfo((Vector2I)fallPoint);
            Vector2 p2Map = LocalToMap(p1.Position);
            Vector2 p1Map = LocalToMap(pointInfo.Position);

            if (p1Map.DistanceTo(p2Map) <= JumpHeight)
            {
                _astarGraph.ConnectPoints(p1.PointID, pointInfo.PointID);
                if (!ignoreDraw) DrawDebugLine(p1.Position, pointInfo.Position, new Color(0, 1, 0, 1));
            }
            else
            {
                _astarGraph.ConnectPoints(p1.PointID, pointInfo.PointID, bidirectional: false);
                if (!ignoreDraw) DrawDebugLine(p1.Position, pointInfo.Position, new Color(1, 1, 0, 1));
            }
        }
    }

	private void ConnectJumpPoints(PointInfo p1, bool ignoreDraw = false)
	{
		foreach(var p2 in _pointInfoList)
		{
			ConnectHorizontalPlatformJumps(p1, p2, ignoreDraw);
            ConnectDiagonalJumpRightEdgeToLeftEdge(p1, p2, ignoreDraw);
            ConnectDiagonalJumpLeftEdgeToRightEdge(p1, p2, ignoreDraw);
			ConnectDropthroughPoints(p1, p2, ignoreDraw);
		}
	}
/* if (p2.IsFallTile
				&& p2.Position.X == p1.Position.X
				&& p2.Position.Y > p1.Position.Y

                && GetCellSourceId(COLLISION_LAYER, (Vector2I)p1.Position) == CELL_IS_EMPTY)*/
    private void ConnectDropthroughPoints(PointInfo p1, PointInfo p2, bool ignoreDraw = false)
    {
		if (p1.IsDropthroughTile)
		{

            var tilePos = LocalToMap(p1.Position);

            tilePos.Y += 1;

            Vector2I? fallPoint = FindDropthroughFallPoint(tilePos);
            if (fallPoint != null)
            {
                var pointInfo = GetPointInfo((Vector2I)fallPoint);
                Vector2 p2Map = LocalToMap(p1.Position);
                Vector2 p1Map = LocalToMap(pointInfo.Position);

                if (p1Map.DistanceTo(p2Map) <= JumpHeight)
                {
                    _astarGraph.ConnectPoints(p1.PointID, pointInfo.PointID);
                    if (!ignoreDraw) DrawDebugLine(p1.Position, pointInfo.Position, new Color(0, 1, 1, 1));
                }
                else
                {
                    _astarGraph.ConnectPoints(p1.PointID, pointInfo.PointID, bidirectional: false);
                   if(!ignoreDraw) DrawDebugLine(p1.Position, pointInfo.Position, new Color(1, 0, 0, 1));
                }
            }

        }
    }

    private void ConnectDiagonalJumpRightEdgeToLeftEdge(PointInfo p1, PointInfo p2, bool ignoreDraw = false)
	{
		if (p1.IsRightEdge)
		{
			Vector2 p1Map = LocalToMap(p1.Position);
			Vector2 p2Map = LocalToMap(p2.Position);

			if (p2.IsLeftEdge
				&& p2.Position.X > p1.Position.X
				//&& p1.Position.X-p2.Position.X<=JumpHeight
				&& p2.Position.Y > p1.Position.Y
				&& p2Map.DistanceTo(p1Map) < JumpDistance)
			{
				_astarGraph.ConnectPoints(p1.PointID, p2.PointID);
                if (!ignoreDraw) DrawDebugLine(p1.Position, p2.Position, new Color(0, 1, 0, 1));
			}
		}
	}
	private void ConnectDiagonalJumpLeftEdgeToRightEdge(PointInfo p1, PointInfo p2, bool ignoreDraw = false)
	{
		if (p1.IsLeftEdge)
		{
			Vector2 p1Map = LocalToMap(p1.Position);
			Vector2 p2Map = LocalToMap(p2.Position);

			if (p2.IsRightEdge
				&& p2.Position.X < p1.Position.X
				&& p2.Position.Y > p1.Position.Y
				//&& p1.Position.X - p2.Position.X <= JumpHeight
				&& p2Map.DistanceTo(p1Map) < JumpDistance)
			{
				_astarGraph.ConnectPoints(p1.PointID, p2.PointID);
                if (!ignoreDraw) DrawDebugLine(p1.Position, p2.Position, new Color(0, 1, 0, 1));
			}
		}
	}
	private void ConnectHorizontalPlatformJumps(PointInfo p1, PointInfo p2, bool ignoreDraw = false)
	{
		if (p1.PointID == p2.PointID) return;

		if(p2.Position.Y == p1.Position.Y && p1.IsRightEdge && p2.IsLeftEdge)
		{
			if(p2.Position.X>p1.Position.X)
			{
				Vector2 p2Map = LocalToMap(p2.Position); 
				Vector2 p1Map = LocalToMap(p1.Position);

				if (p2Map.DistanceTo(p1Map) < JumpDistance + 1)
				{
					_astarGraph.ConnectPoints(p1.PointID, p2.PointID);
                    if (!ignoreDraw) DrawDebugLine(p1.Position, p2.Position, new Color(0,1,0,1));
				}
			}
		}
	}

	private void ConnectHorizontalPoints(PointInfo p1, bool ignoreDraw = false)
	{
		if (p1.IsLeftEdge || p1.IsLeftWall || p1.IsFallTile || p1.IsDropthroughTile || p1.IsDropthroughFallTile )
		{
			PointInfo closest = null;

			foreach (var p2 in _pointInfoList)
			{
				if (p1.PointID == p2.PointID) { continue; }

				if ((p2.IsRightEdge || p2.IsRightWall || p2.IsFallTile || p2.IsDropthroughTile || p2.IsDropthroughFallTile ) && p2.Position.Y == p1.Position.Y && p2.Position.X > p1.Position.X)
				{
					

					if (closest == null)
					{
						closest = new PointInfo(p2.PointID, p2.Position);
					}

					if (p2.Position.X < closest.Position.X)
					{
						closest.Position = p2.Position;
						closest.PointID = p2.PointID;
					}
				}
			}
			
			if (closest != null)
			{
				if (HorizontalConnectionCanBeMade((Vector2I)p1.Position, (Vector2I)closest.Position))
				{
					

					_astarGraph.ConnectPoints(p1.PointID, closest.PointID);
					if(!ignoreDraw)DrawDebugLine(p1.Position, closest.Position, new Color(0, 1, 0, 1));
				}
			}
		}
	}

	private bool HorizontalConnectionCanBeMade(Vector2I p1, Vector2I p2)
	{
		Vector2I startScan = LocalToMap(p1);
		Vector2I endScan = LocalToMap(p2);

		for (int i = startScan.X; i < endScan.X; ++i)
		{
			if (GetCellSourceId(COLLISION_LAYER, new Vector2I(i, startScan.Y)) != CELL_IS_EMPTY || GetCellSourceId(COLLISION_LAYER, new Vector2I(i, startScan.Y + 1)) == CELL_IS_EMPTY)
			{
				{
					return false;
				}

			}

		}

			return true;
	}

	#endregion

	#region Tile Fall Points

	private Vector2I? GetStartScanTileForFallPoint(Vector2I tile, bool isLeft)
	{
		//var tileAbove = new Vector2I(tile.X, tile.Y - 1);
		var point = GetPointInfo(tile);


		if (point == null) return null;
		
		var tileScan = Vector2I.Zero;

		
		if (isLeft)
		{
			tileScan = new Vector2I(tile.X - 1, tile.Y - 1);
			return tileScan;
		}
		else if (!isLeft)
		{
			tileScan = new Vector2I(tile.X + 1, tile.Y - 1);
			return tileScan;
		}


		return null;

	}
   

	private Vector2I? FindFallPoint(Vector2 tile, bool isLeft)
	{
		var scan = GetStartScanTileForFallPoint((Vector2I)tile, isLeft);
		if(scan == null ) return null;

		var tileScan = (Vector2I)scan;
		Vector2I? fallTile= null;

		for (int i = 0; i<MAX_TILE_FALL_SCAN_DEPTH; ++i)
		{
			if(GetCellSourceId(COLLISION_LAYER, new Vector2I(tileScan.X, tileScan.Y+1))!= CELL_IS_EMPTY)
			{
				fallTile = tileScan; 
				break;
			}
			tileScan.Y++;
		}
		return fallTile;
	}

	private Vector2I?[] FindFallPoints(Vector2I tile)
	{
        System.Collections.Generic.List<Vector2I?> fallTiles = new();
		var mapToLocal = MapToLocal(tile);
		var tileAbove = new Vector2I(tile.X, tile.Y - 1);
		if (GetPointInfo(tileAbove) != null)
		{

			if (GetPointInfo(tileAbove).IsLeftEdge)
			{
				fallTiles.Add(FindFallPoint(tileAbove, true));
			}
			if (GetPointInfo(tileAbove).IsRightEdge)
			{
				fallTiles.Add(FindFallPoint(tileAbove, false));
			}

		}
		return fallTiles.ToArray();
    }

	private void AddFallPoint(Vector2I tile)
	{


		//if (FindFallPoints(tile) == null) return;

		foreach(var fallTile in FindFallPoints(tile))
		{
			if(fallTile != null)
			{
				CreateFallPoint((Vector2I)fallTile);
			}
		}
		

		

        


	}

	private void CreateFallPoint(Vector2I fallTile)
	{
		var fallTileLocal = (Vector2I)MapToLocal((Vector2I)fallTile);

		long existingPointId = TileAlreadyExistInGraph((Vector2I)fallTile);

        if (existingPointId == -1)
        {
            long pointId = _astarGraph.GetAvailablePointId();
            var pointInfo = new PointInfo(pointId, fallTileLocal);
            pointInfo.IsDropthroughFallTile = true;
            _pointInfoList.Add(pointInfo);
            _astarGraph.AddPoint(pointId, fallTileLocal);
            AddVisualPoint((Vector2I)fallTile, new Color(1, 0.35f, 0.1f, 1), scale: 0.35f);
        }
        else
        {
            _pointInfoList.Single(x => x.PointID == existingPointId).IsDropthroughFallTile = true;
            AddVisualPoint((Vector2I)fallTile, new Color("#ef7d57"), scale: 0.30f);
        }
    }

    private Vector2I? GetStartScanTileForDropthroughFallPoint(Vector2I tile)
    {
        var tileAbove = new Vector2I(tile.X, tile.Y - 1);
        var point = GetPointInfo(tileAbove);


        if (point == null) return null;

        var tileScan = Vector2I.Zero;

        if (point.IsDropthroughTile)
        {

            tileScan = new Vector2I(tile.X, tile.Y );
            return tileScan;
        }
        


        return null;

    }


    private Vector2I? FindDropthroughFallPoint(Vector2 tile)
    {
        var scan = GetStartScanTileForDropthroughFallPoint((Vector2I)tile);
        if (scan == null) return null;

        var tileScan = (Vector2I)scan;
        Vector2I? fallTile = null;

        for (int i = 0; i < MAX_TILE_FALL_SCAN_DEPTH; ++i)
        {
            if (GetCellSourceId(COLLISION_LAYER, new Vector2I(tileScan.X, tileScan.Y + 1)) != CELL_IS_EMPTY)
            {
                fallTile = tileScan;
                break;
            }
            tileScan.Y++;
        }
        return fallTile;
    }

    private void AddDropthroughFallPoint(Vector2I tile)
    {
        Vector2I? fallTile = FindDropthroughFallPoint(tile);
        if (fallTile == null) return;
        var fallTileLocal = (Vector2I)MapToLocal((Vector2I)fallTile);

        long existingPointId = TileAlreadyExistInGraph((Vector2I)fallTile);

        if (existingPointId == -1)
        {
            long pointId = _astarGraph.GetAvailablePointId();
            var pointInfo = new PointInfo(pointId, fallTileLocal);
            pointInfo.IsDropthroughFallTile = true;
            _pointInfoList.Add(pointInfo);
            _astarGraph.AddPoint(pointId, fallTileLocal);
            AddVisualPoint((Vector2I)fallTile, new Color(1, 1f, 0f, 1), scale: 0.16f);
        }
        else
        {
            _pointInfoList.Single(x => x.PointID == existingPointId).IsDropthroughFallTile = true;
            AddVisualPoint((Vector2I)fallTile, new Color(1,1,0,1), scale: 0.11f);
        }
    }
    #endregion

    #region Tile Edge & Wall Graph Points
    private void AddLeftEdgePoint(Vector2I tile)
	{
		if (TileAboveExist(tile))
		{
			return;
		}

		if(GetCellSourceId(COLLISION_LAYER, new Vector2I(tile.X - 1, tile.Y)) == CELL_IS_EMPTY)
		{
			var tileAbove = new Vector2I(tile.X, tile.Y-1);

			long existingPointId = TileAlreadyExistInGraph(tileAbove);

			if(existingPointId == -1)
			{
				long pointId = _astarGraph.GetAvailablePointId();
				var pointInfo = new PointInfo(pointId, (Vector2I)MapToLocal(tileAbove));
				pointInfo.IsLeftEdge = true;
				_pointInfoList.Add(pointInfo);
				_astarGraph.AddPoint(pointId, (Vector2I)MapToLocal(tileAbove));
				AddVisualPoint(tileAbove);
			}
			else
			{
				_pointInfoList.Single(x => x.PointID == existingPointId).IsLeftEdge = true;
				AddVisualPoint(tileAbove, new Color("#a73eff"));
			}
		}
		

	}
	private void AddRightEdgePoint(Vector2I tile)
	{
		if (TileAboveExist(tile))
		{
			return;
		}

		if(GetCellSourceId(COLLISION_LAYER, new Vector2I(tile.X + 1, tile.Y)) == CELL_IS_EMPTY )
		{
			var tileAbove = new Vector2I(tile.X, tile.Y-1);

			long existingPointId = TileAlreadyExistInGraph(tileAbove);

			if(existingPointId == -1)
			{
				long pointId = _astarGraph.GetAvailablePointId();
				var pointInfo = new PointInfo(pointId, (Vector2I)MapToLocal(tileAbove));
				pointInfo.IsRightEdge = true;
				_pointInfoList.Add(pointInfo);
				_astarGraph.AddPoint(pointId, (Vector2I)MapToLocal(tileAbove));
				AddVisualPoint(tileAbove, new Color("#94b0c2"));
			}
			else
			{
				_pointInfoList.Single(x => x.PointID == existingPointId).IsRightEdge = true;
				AddVisualPoint(tileAbove, new Color("#ffcd75"), scale:0.8f);
			}
		}
		

	}
	private void AddLeftWallPoint(Vector2I tile)
	{
		if (TileAboveExist(tile))
		{
			return;
		}

		if(GetCellSourceId(COLLISION_LAYER, new Vector2I(tile.X - 1, tile.Y - 1)) != CELL_IS_EMPTY)
		{
			var tileAbove = new Vector2I(tile.X, tile.Y-1);

			long existingPointId = TileAlreadyExistInGraph(tileAbove);

			if(existingPointId == -1)
			{
				long pointId = _astarGraph.GetAvailablePointId();
				var pointInfo = new PointInfo(pointId, (Vector2I)MapToLocal(tileAbove));
				pointInfo.IsLeftWall = true;
				_pointInfoList.Add(pointInfo);
				_astarGraph.AddPoint(pointId, (Vector2I)MapToLocal(tileAbove));
				AddVisualPoint(tileAbove, new Color(0,0,1,1));
			}
			else
			{
				_pointInfoList.Single(x => x.PointID == existingPointId).IsLeftWall = true;
				AddVisualPoint(tileAbove, new Color(0,0,0,1));
			}
		}
		

	}
	private void AddRightWallPoint(Vector2I tile)
	{
        // If a tile exist above, it's not an edge
        if (TileAboveExist(tile))
		{
			return;
		}

        // If the tile to the up-right (X + 1, Y -1) is not empty
        if (GetCellSourceId(COLLISION_LAYER, new Vector2I(tile.X + 1, tile.Y-1)) != CELL_IS_EMPTY)
		{
			var tileAbove = new Vector2I(tile.X, tile.Y - 1);

            long existingPointId = TileAlreadyExistInGraph(tileAbove);

			if(existingPointId == -1)
			{
				long pointId = _astarGraph.GetAvailablePointId();
				var pointInfo = new PointInfo(pointId, (Vector2I)MapToLocal(tileAbove));
				pointInfo.IsRightWall = true;
				_pointInfoList.Add(pointInfo);
				_astarGraph.AddPoint(pointId, (Vector2I)MapToLocal(tileAbove));
				AddVisualPoint(tileAbove, new Color(0,0,0,1));
			}
			else
			{
				_pointInfoList.Single(x => x.PointID == existingPointId).IsRightWall = true;
				AddVisualPoint(tileAbove, new Color("566cB6"), 0.65f);
			}
		}
		

	}
	

	private void AddRightDropThroughStairPoints(Vector2I tile)
	{
		// If a tile exist above, it's not an edge
		if (TileAboveExist(tile))
		{
			return;
		}

		PointInfo rightTile = GetPointInfo(LocalToMap(tile));


        if (rightTile != null)
		{
			/*GD.Print("Got point info");
			GD.Print(LocalToMap(rightTile.Position));
			rightTile.PrintInfo();*/

			if (GetCellSourceId(COLLISION_LAYER, LocalToMap(rightTile.Position)) != CELL_IS_EMPTY && rightTile.IsDropthroughTile)
			{
				//GD.Print("YOU FOUND SOMETHING");
				var tileAbove = new Vector2I(tile.X, tile.Y - 1);

				long existingPointId = TileAlreadyExistInGraph(tileAbove);

			}
		}

    }

    //need to fix the "not set to an instance of an object" problem

    private void AddDropthroughPoint(Vector2I tile)
	{
        // If a tile exist above, it's not an edge
        if (TileAboveExist(tile))
        {
            return;
        }

		var tileData = GetCellTileData(0, tile);

		if (tileData == null)
			return;


		if ((bool)tileData.GetCustomData("DropThroughTiles"))
		{
			//Correctly being called
            var tileAbove = new Vector2I(tile.X, tile.Y - 1);

            long existingPointId = TileAlreadyExistInGraph(tileAbove);

            if (existingPointId == -1)
            {
                long pointId = _astarGraph.GetAvailablePointId();
                var pointInfo = new PointInfo(pointId, (Vector2I)MapToLocal(tileAbove));
                pointInfo.IsDropthroughTile = true;
                _pointInfoList.Add(pointInfo);
                _astarGraph.AddPoint(pointId, (Vector2I)MapToLocal(tileAbove));
                AddVisualPoint(tileAbove, new Color(1, 0, 1, 1), 0.25f);
            }
            else
            {
				
					_pointInfoList.Single(x => x.PointID == existingPointId).IsDropthroughTile = true;
					AddVisualPoint(tileAbove, new Color(1, 0, 1, 1), 0.20f);
				
            }
        }
		


    }




    public void AddVisualPoint(Vector2I tile, Color? color = null, float scale = 1.0f, Pathfinder pathfinder = null)
	{
		if(!ShowDebugGraph) return;
		GraphPoint visualPoint = _graphPoint.Instantiate() as GraphPoint;

		PointInfo point = GetPointInfo(tile);
		

		if (visualPoint != null)
		{
            
            if (pathfinder != null)
			{
				visualPoint.pathfinder = pathfinder;
			}
			if (color != null)
			{
				visualPoint.Modulate = (Color)color;
			}

			if (scale != 1.0f && scale > 0.1f)
			{
				visualPoint.Scale = new Vector2(scale, scale);
			}

		
			visualPoint.Position = MapToLocal(tile);

			//GD.Print("Placed Visual Point at " + LocalToMap(visualPoint.Position));
			
			AddChild(visualPoint);
			
		}
	}

	public long TileAlreadyExistInGraph(Vector2I tile)
	{
		var localPos = MapToLocal(tile);

		if (_astarGraph.GetPointCount() > 0)
		{
			var pointId = _astarGraph.GetClosestPoint(localPos);

			if (_astarGraph.GetPointPosition(pointId) == localPos)
			{
				return pointId;
			}
		}

		return -1;
	}
	private bool TileAboveExist(Vector2I tile)
	{
		if(GetCellSourceId(COLLISION_LAYER, new Vector2I(tile.X, tile.Y-1)) == CELL_IS_EMPTY)
		{
			return false;
		}
		return true;
	}

    #endregion

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
