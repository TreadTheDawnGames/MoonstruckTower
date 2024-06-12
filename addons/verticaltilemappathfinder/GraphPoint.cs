using Godot;
using System;

public partial class GraphPoint : Sprite2D
{
	


	public Pathfinder pathfinder;
	public Timer timer;
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


    void Destroy()
	{
		try
		{

		QueueFree();
		}catch 
		{
			//GD.Print(ex.Message);
		}
	}
}
