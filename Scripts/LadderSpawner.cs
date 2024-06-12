using Godot;
using System;

public partial class LadderSpawner : Node2D, ITool
{
    public bool useRelease { get; } = false;
    public string name { get; } = "Ladder Spawner";

    public bool charged { get; } = false;

    public bool animating { get; private set; } = false;

    AnimatedSprite2D linkSprite;
    player link;
    Area2D ladderGrabber;
    PackedScene ladderScene;
    Marker2D ladderSpawnpoint;

    bool ladderPlaced = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        ladderScene = GD.Load<PackedScene>("res://Scenes/tool_ladder.tscn");
        ladderGrabber = GetNode<Area2D>("LadderGrabber");
        ladderSpawnpoint = GetNode<Marker2D>("LadderSpawnpoint");
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


    public void SetupTool(AnimatedSprite2D character, player playerLink)
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
                    PickupLadder(area);
                    return;
                }
            }
        }
        else
        {
            PlaceLadder();

        }
    }
    private void PickupLadder(Area2D ladder)
    {
        GD.Print("Picked up Ladder");
        ladder.Owner.QueueFree();
        ladderPlaced = false;
    }

    private void PlaceLadder()
    {
        GD.Print("Placed Ladder");

        ladderSpawnpoint.Set("position", linkSprite.FlipH ? new Vector2(7,8) : new Vector2(-7,8)) ;

        Ladder ladder = ladderScene.Instantiate<Ladder>();

        ladder.GlobalPosition = ladderSpawnpoint.GlobalPosition;
        ladderPlaced = true;

        GetTree().Root.AddChild(ladder);

    }
}
