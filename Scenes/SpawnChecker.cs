using Godot;
using System;
using System.Linq;

public partial class SpawnChecker : Area2D
{

	Player player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        player = (Player)GetTree().GetFirstNodeInGroup("Player");

		//if in boss room
		if(PlayerPrefs.GetInt("PositionY", -99) < -7338)
		{
			GlobalPosition = new Vector2(120, -7155);
		}
		else
		{
	        GlobalPosition = new Vector2(PlayerPrefs.GetInt("PositionX", 166), PlayerPrefs.GetInt("PositionY", -99)-1);
		}


		if (PlayerPrefs.GetBool("LadderPlaced", false))
		{
            foreach (LadderSpawner ls in player.toolBag.GetChildren().OfType<LadderSpawner>())
            {
                ls.ladderPlaced = true;
            }

            GD.Print("Placing Ladder");
			PackedScene ladderScene = GD.Load<PackedScene>("res://Scenes/Tools/Ladder/tool_ladder.tscn");
			var ladder = ladderScene.Instantiate<Ladder>();

			ladder.GlobalPosition = new Vector2(PlayerPrefs.GetInt("LadderPositionX"), PlayerPrefs.GetInt("LadderPositionY"));
			ladder.Rotation = PlayerPrefs.GetFloat("LadderRotation");
            ladder.AddToGroup("Ladders");

            GetTree().Root.GetChild<Node2D>(0).CallDeferred("add_child", ladder);
		}
		
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		if (HasOverlappingBodies())
		{
			
			GlobalPosition += new Vector2(0, 16);
		}
		else
		{
			player.GlobalPosition = GlobalPosition;

			
            Camera2D camera = (Camera2D)GetTree().GetFirstNodeInGroup("Camera");
			Vector2 camStartPos = GlobalPosition;
			camStartPos.Y += -44;

			camera.GlobalPosition = camStartPos;

			DebugTools game = GetParent<DebugTools>();
			game.fader.FadeIn();



			QueueFree();
		
		}
	}
}
