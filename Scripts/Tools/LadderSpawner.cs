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
    Area2D roofSpawnCheck;
    Area2D wallSpawnCheck;
    PackedScene ladderScene;
    Marker2D ladderSpawnpoint;
    Marker2D ladderSpawnpoint2;

    public bool canUse = true;

    public bool ladderPlaced = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        ladderScene = GD.Load<PackedScene>("res://Scenes/Tools/Ladder/tool_ladder.tscn");
        ladderGrabber = GetNode<Area2D>("LadderGrabber");
        ladderSpawnpoint = GetNode<Marker2D>("LadderSpawnpoint");
        ladderSpawnpoint2 = GetNode<Marker2D>("LadderSpawnpoint2");
        roofSpawnCheck = GetNode<Area2D>("RoofCheck");
        wallSpawnCheck = GetNode<Area2D>("WallCheck");
        
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
        
        if(canUse) 
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
                if (area.Owner is Ladder)
                {
                    PickupLadder((Ladder)area.Owner);
                    return;
                }
            }
        }
        else
        {
            if(!roofSpawnCheck.HasOverlappingBodies())
                PlaceLadder();

        }
    }
    private void PickupLadder(Ladder ladder)
    {
        GD.Print("Picked up Ladder");
        ladder.Despawn(false);
        ladderPlaced = false;
    }

    private void PlaceLadder()
    {
        GD.Print("Placed Ladder");


        Ladder ladder = ladderScene.Instantiate<Ladder>();

        if (!wallSpawnCheck.HasOverlappingBodies())
        {
            ladder.GlobalPosition = ladderSpawnpoint.GlobalPosition;
        }
        else
        {
            ladder.GlobalPosition = ladderSpawnpoint2.GlobalPosition;

        }

        //maybe add: if roof hit try placing rotated 90. if not, don't spawn.

        ladderPlaced = true;

        ladder.AddToGroup("Ladders");

        GetTree().Root.GetNode("Game").AddChild(ladder);

    }
}
