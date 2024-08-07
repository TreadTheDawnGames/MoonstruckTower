using Godot;
using System;

public partial class GraphPoint : Sprite2D
{



	public Pathfinder pathfinder;
	public Timer timer;
    PackedScene _graphPoint = ResourceLoader.Load<PackedScene>("res://addons/VerticalTileMapPathFinder/GraphPoint.tscn");
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		timer = GetNode<Timer>("Timer");
		timer.Stop();
		timer.Timeout += () => Destroy();
	}

    
    public override void _PhysicsProcess(double delta)
    {
		try
		{

			base._PhysicsProcess(delta);
			if (pathfinder != null)
			{

				if (pathfinder.Position.DistanceTo(Position) <= 10)
				{
					Destroy();
				}
			}
		}catch
		{
			
		}
    }

    public static void AddVisualPoint(Vector2 position, Node parent, Color? color = null, float scale = 1.0f, float timer = 0)
    {
        GraphPoint visualPoint = ResourceLoader.Load<PackedScene>("res://addons/VerticalTileMapPathFinder/GraphPoint.tscn").Instantiate() as GraphPoint;



        if (visualPoint != null)
        {

            
            if (color != null)
            {
                visualPoint.Modulate = (Color)color;
            }

            if (scale != 1.0f && scale > 0.1f)
            {
                visualPoint.Scale = new Vector2(scale, scale);
            }



            visualPoint.Position = position;



            parent.GetTree().Root.AddChild(visualPoint);
            if (timer > 0)
            {
                visualPoint.timer.WaitTime = timer;
                visualPoint.timer.Start();
            }

        }
    }

    void Destroy()
	{
		try
		{
		    QueueFree();
		}
        catch (Exception e) 
		{
			GD.Print(e.Message);
		}
	}
}
