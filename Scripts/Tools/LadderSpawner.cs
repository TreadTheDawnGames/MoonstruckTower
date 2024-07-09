using Godot;
using System;

public partial class LadderSpawner : Node2D, ITool
{
    public bool useRelease { get; } = false;
    public string name { get; } = "Ladder Spawner";

    public bool charged { get; } = false;

    public bool animating { get; private set; } = false;



    [Export]
    public Texture2D displayTexture { get; private set; }

    AnimatedSprite2D linkSprite;
    Player link;
    Area2D ladderGrabber;
    Area2D ladderSpawnCheck;
    PackedScene ladderScene;
    Marker2D ladderSpawnpoint;

    bool ladderPlaced = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        ladderScene = GD.Load<PackedScene>("res://Scenes/Tools/Ladder/tool_ladder.tscn");
        ladderGrabber = GetNode<Area2D>("LadderGrabber");
        ladderSpawnpoint = GetNode<Marker2D>("LadderSpawnpoint");
        ladderSpawnCheck = GetNode<Area2D>("SpawnCheck");
        
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    void AnimationFinished()
    {
        if(linkSprite.Animation == "LadderPlace")
        {
            animating = false;
            link.usingTool = false;
        }
    }

    public bool Identify()
    {
        GD.Print("I am LadderSpawner");
        return true;
    }

    public void Use(Vector2 direction)
    {



            animating = true;
            linkSprite.Play("LadderPlace");
        
            HandleLadder();

            
        
            link.usingTool = false;
    }


    public void SetupTool(AnimatedSprite2D character, Player playerLink)
    {
        if(link == null)
        {
            link = playerLink;
        }
        if (linkSprite == null)
        {
            GD.Print("Ladder Animator Setup");
            linkSprite = character;
            linkSprite.AnimationFinished += () => AnimationFinished();
        }
    }

    

    private void HandleLadder()
    {
        if (ladderPlaced)
        {

            foreach (Area2D area in ladderGrabber.GetOverlappingAreas())
            {
                if (area.Owner.Name == "ToolLadder")
                {
                    PickupLadder((Ladder)area.Owner);
                    return;
                }
            }
        }
        else
        {
            if (!ladderSpawnCheck.HasOverlappingBodies())
                PlaceLadder();

        }
    }
    private void PickupLadder(Ladder ladder)
    {
        GD.Print("Picked up Ladder");
        ladder.Despawn();
        ladderPlaced = false;
    }

    private void PlaceLadder()
    {
        GD.Print("Placed Ladder");


        Ladder ladder = ladderScene.Instantiate<Ladder>();

        ladder.GlobalPosition = ladderSpawnpoint.GlobalPosition;
        ladderPlaced = true;

        GetTree().Root.GetNode("Game").AddChild(ladder);

    }
}
