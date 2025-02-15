using Godot;
using System;
using System.Reflection.Metadata;

public partial class Bow : Node, ITool
{
    public bool useRelease { get; } = true;
    public bool charged { get; private set; } = false;
    public string name { get; } = "Bow";

    [Export]
    public Texture2D displayTexture { get; private set; }

    int arrowCount = 99999;
	PackedScene projectile;
	Marker2D arrowSpawnPoint;
    Area2D spawnArea;
	AnimatedSprite2D salmonBoySprite;
    Player PlayerChar;
    AudioPlayer audioPlayer;
    [Export]
    AudioStream drawSound, shootSound;

    bool shouldSound = true;

    public bool animating {  get; private set; } 
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        audioPlayer = GetNode<AudioPlayer>("AudioStreamPlayer2D");
        projectile = GD.Load<PackedScene>("res://Scenes/Tools/Bow/arrow.tscn");
		arrowSpawnPoint = GetNode<Marker2D>(new NodePath("BowSpawner"));
        spawnArea = GetNode<Area2D>("BowSpawner/Area2D");
	}

    void Charge()
    {
        if (salmonBoySprite.Animation.ToString().Contains("Draw") && shouldSound)
        {
            audioPlayer.PlaySound(drawSound);
            shouldSound = false;
        }


        if (salmonBoySprite.Animation == "BowDraw" || salmonBoySprite.Animation == "BowDrawWalk")
        {
            charged = true;
            salmonBoySprite.Play("BowHold");
        }
        else if (salmonBoySprite.Animation == "BowDrawVert" || salmonBoySprite.Animation == "BowDrawWalkVert")
        {
            charged = true;
            salmonBoySprite.Play("BowHoldVert");
        }
        else if (salmonBoySprite.Animation == "BowHold" || salmonBoySprite.Animation == "BowHoldVert")
        {
            animating = false;
        }
    }

    void ShootH()
    {
        if (spawnArea.GetOverlappingBodies().Count == 0)
        {
            bool flipped = salmonBoySprite.FlipH;


            arrowSpawnPoint.Set("position", new Vector2(-6, 0));

            var arrow = projectile.Instantiate<Projectile>();
            arrow.shootDirection = Vector2.Right;

            arrow.GlobalPosition = arrowSpawnPoint.GlobalPosition;
            arrow.GlobalRotation = flipped ? Mathf.DegToRad(180f) : Mathf.DegToRad(0f);
            arrow.speed *= flipped ? 1 : -1;

            GetTree().Root.GetChild<Node2D>(0).AddChild(arrow);

        }
        ////GD.Print("Used
        //");
        PlayerChar.usingTool = false;
            charged = false;
    }
    void ShootV()
    {

       
            arrowSpawnPoint.Set("position", new Vector2(0, -6));

            var arrow = projectile.Instantiate<Projectile>();
            arrow.shootDirection = Vector2.Up;
            arrow.GlobalPosition = arrowSpawnPoint.GlobalPosition;
            arrow.GlobalRotation = Mathf.DegToRad(90f);

            GetTree().Root.AddChild(arrow);

        
            PlayerChar.usingTool = false;
            charged = false;
    }
	

	public bool Identify()
	{
		////GD.Print("I am Bow");
		return true;
	}

    public void Use(Vector2 direction)
    {
        if (charged)
        {
                audioPlayer.PlaySound(shootSound);
            shouldSound = true;
            if (salmonBoySprite.Animation == "BowHoldVert" || salmonBoySprite.Animation == "BowWalkVert")
            {
                salmonBoySprite.Play("BowShootVert");
                ShootV();
            }
            else if (salmonBoySprite.Animation == "BowHold" || salmonBoySprite.Animation == "BowWalk")
            {
                salmonBoySprite.Play("BowShoot");
                ShootH();
            }

        }

    }

    public void PreUse(Vector2 direction)
    {

        bool isWalking = direction.X != 0 ? true : false;
        if (isWalking)
        {
            if (direction.Y < 0)
            {
                salmonBoySprite.Play("BowDrawWalkVert");
            }
            else
            {
                salmonBoySprite.Play("BowDrawWalk");
            }
        }
        else if (direction.Y < 0)
        {
            salmonBoySprite.Play("BowDrawVert");
        }
        else
        {
            salmonBoySprite.Play("BowDraw");
        }
            animating = true;
    }

    public void WalkWhileUseAnim(Vector2 direction)
    {
        if(direction.Y < 0)
        {
            salmonBoySprite.Play("BowWalkVert");
        }
        else
        {
            salmonBoySprite.Play("BowWalk");
        }
    }

    public void UpdateUseDirection(Vector2 direction) 
    {
        if (direction.Y < 0 && direction.X == 0)
        {
            salmonBoySprite.Play("BowHoldVert");
        }
        else if (direction.Y >= 0 && direction.X == 0)
        {
            salmonBoySprite.Play("BowHold");
        }
        

    }

    public void SetupTool(AnimatedSprite2D character, Player playerChar)
    {
        if (PlayerChar == null)
        {
            PlayerChar = playerChar;
        }
        if (salmonBoySprite == null)
        {
            ////GD.Print("Bow Animator Setup");
            salmonBoySprite = character;
            salmonBoySprite.AnimationFinished += () => Charge();

        }

    }

    public void BecomeActiveTool()
    {
        charged = false;
        shouldSound = true;
    }

  
}
