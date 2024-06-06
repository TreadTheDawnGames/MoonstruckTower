using Godot;
using System;
using Godot.Collections;
using System.Net.Sockets;
using System.Linq;
using System.Transactions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

public class PointInfo
{
	public bool IsFallTile;
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
}

public partial class TileMapPathFind : TileMap
{
	[Export]
	public bool ShowDebugGraph = true;
	[Export]
	public int JumpDistance = 6;
	[Export]
	public int JumpHeight = 4;

	private const int COLLISION_LAYER = 0;
	private const int CELL_IS_EMPTY = -1;
	private const int MAX_TILE_FALL_SCAN_DEPTH = 500;

	private AStar2D _astarGraph = new AStar2D();
	private Array<Vector2I> _usedTiles;
	private PackedScene _graphPoint;

	private System.Collections.Generic.List<PointInfo> _pointInfoList = new System.Collections.Generic.List<PointInfo> ();

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
		}
	}
	private void DrawDebugLine(Vector2 to, Vector2 from, Color color)
	{
		if(ShowDebugGraph)
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

    public override void _Draw()
    {
		if (ShowDebugGraph)
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
		}
	}

	private void ConnectJumpPoints(PointInfo p1)
	{
		foreach(var p2 in _pointInfoList)
		{
			ConnectHorizontalPlatformJumps(p1, p2);
		}
	}

	private void ConnectHorizontalPlatformJumps(PointInfo p1, PointInfo p2)
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
					DrawDebugLine(p1.Position, p2.Position, new Color(0,1,0,1));
				}
			}
		}
	}

	private void ConnectHorizontalPoints(PointInfo p1)
	{
		if (p1.IsLeftEdge || p1.IsLeftWall || p1.IsFallTile)
		{
			PointInfo closest = null;

			foreach (var p2 in _pointInfoList)
			{
				if (p1.PointID == p2.PointID) { continue; }

				if ((p2.IsRightEdge || p2.IsRightWall || p2.IsFallTile) && p2.Position.Y == p1.Position.Y && p2.Position.X > p1.Position.X)
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
				if (!HorizontalConnectionCannotBeMade((Vector2I)p1.Position, (Vector2I)closest.Position))
				{

					_astarGraph.ConnectPoints(p1.PointID, closest.PointID);
					DrawDebugLine(p1.Position, closest.Position, new Color(0, 1, 0, 1));
				}
			}
		}
	}

	private bool HorizontalConnectionCannotBeMade(Vector2I p1, Vector2I p2)
	{
		Vector2I startScan = LocalToMap(p1);
		Vector2I endScan = LocalToMap(p2);

		for (int i = startScan.X; i < endScan.X; ++i)
		{
			if (GetCellSourceId(COLLISION_LAYER, new Vector2I(i, startScan.Y)) != CELL_IS_EMPTY || GetCellSourceId(COLLISION_LAYER, new Vector2I(i, startScan.Y + 1)) == CELL_IS_EMPTY)
			{
				{
					return true;
				}

			}

		}

			return false;
	}

    #endregion

    #region Tile Fall Points
    private Vector2I? GetStartScanTileForFallPoint(Vector2I tile)
	{
		var tileAbove = new Vector2I(tile.X , tile.Y-1);
		var point = GetPointInfo(tileAbove);

		if (point == null) return null;

		var tileScan = Vector2I.Zero;

		if (point.IsLeftEdge)
		{
			tileScan = new Vector2I(tile.X - 1, tile.Y - 1);
			return tileScan;
		}else if (point.IsRightEdge)
		{
			tileScan = new Vector2I(tile.X + 1, tile.Y - 1);
			return tileScan;
		}

		return null;

	}
    #endregion

	private Vector2I? FindFallPoint(Vector2 tile)
	{
		var scan = GetStartScanTileForFallPoint((Vector2I)tile);
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

	private void AddFallPoint(Vector2I tile)
	{
		Vector2I? fallTile = FindFallPoint(tile);
		if(fallTile == null ) return;
		var fallTileLocal = (Vector2I)MapToLocal((Vector2I)fallTile);

		long existingPointId = TileAlreadyExistInGraph((Vector2I)fallTile);

		if(existingPointId == -1)
		{
			long pointId = _astarGraph.GetAvailablePointId();
			var pointInfo = new PointInfo(pointId, fallTileLocal);
			pointInfo.IsFallTile = true;
			_pointInfoList.Add(pointInfo);
			_astarGraph.AddPoint(pointId, fallTileLocal);
			AddVisualPoint((Vector2I)fallTile, new Color(1, 0.35f, 0.1f, 1), scale: 0.35f);
		}
		else
		{
			_pointInfoList.Single(x=>x.PointID == existingPointId).IsFallTile = true;
			AddVisualPoint((Vector2I)fallTile, new Color("#ef7d57"), scale: 0.30f);
		}
	}

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

		if(GetCellSourceId(COLLISION_LAYER, new Vector2I(tile.X + 1, tile.Y)) == CELL_IS_EMPTY)
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
				AddVisualPoint(tileAbove, new Color("#ffcd75"));
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
		if (TileAboveExist(tile))
		{
			return;
		}

		if(GetCellSourceId(COLLISION_LAYER, new Vector2I(tile.X + 1, tile.Y-1)) != CELL_IS_EMPTY)
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
	private void AddVisualPoint(Vector2I tile, Color? color = null, float scale = 1.0f)
	{
		if(!ShowDebugGraph) return;

		Sprite2D visualPoint = _graphPoint.Instantiate() as Sprite2D;

		if(color != null)
		{
			visualPoint.Modulate = (Color)color;
		}

		if(scale != 1.0f && scale > 0.1f)
		{
			visualPoint.Scale = new Vector2(scale, scale);
		}
		visualPoint.Position = MapToLocal(tile);
		AddChild(visualPoint);
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
