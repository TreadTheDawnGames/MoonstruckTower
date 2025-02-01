using Godot;
using System;
using System.Linq;

public partial class SoftlockPrevention_UnlockIfInArea : Area2D
{
	[Export]
	Door theDoor;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        DebugTools game = (DebugTools)GetTree().GetFirstNodeInGroup("Game");
		game.SpawnFinished += DoLogic;
	}

	void DoLogic()
	{
        if (GetOverlappingAreas().Where(a => a is SpawnChecker).Any())
        {
			foreach (var l in theDoor.lockList)
				l.UnlockMe(null);
			theDoor.AttemptToOpen();
            GD.Print("SpawnCheckerFound");
        }
        else
        {
            GD.Print("NOT FOUND");
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
