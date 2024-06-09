using Godot;
using System;

public partial class GraphPoint : Sprite2D
{
	


	public Pathfinder pathfinder;
    // Called when the node enters the scene tree for the first time.
    /*public override void _Ready()
	{
		if(pathfinder!=null)
			pathfinder.ReachedPoint += () => Destroy();
	}*/
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
		if (pathfinder != null)
		{

			if (pathfinder.Position.DistanceTo(Position)<=10)
			{
				Destroy();
			}
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
