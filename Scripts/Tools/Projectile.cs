using Godot;
using System;
using System.IO;
using System.Linq;
using static Godot.WebSocketPeer;
using System.Reflection.Emit;
using System.Threading.Channels;

public partial class Projectile : CharacterBody2D
{
	[Export] public float speed = 300;
	[Export] public Vector2 shootDirection;
	[Export] bool destroyOnContact = false;
	protected Area2D collisionBox;
    protected HitBox2D hitBox;
	protected bool fallDown = false;
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	AudioPlayer audioPlayer;
	[Export]
	AudioStream impactSound;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		collisionBox = GetNode<Area2D>("CollisionBox");
		hitBox = GetNode<HitBox2D>("HitBox2D");
		audioPlayer = GetNode<AudioPlayer>("AudioStreamPlayer2D");
		

		collisionBox.BodyEntered += (node) => HitWorld(node);
        collisionBox.BodyExited += (node) => fallDown = true;
    }

    

    protected virtual void HitWorld(Node2D node)
	{

		GD.Print(node.GetType());
		GD.Print();

        /*if (node.GetType() == typeof(HurtBox2D))
		{
			HitHurtBox();
			
			return;
        }*/
		//CallDeferred("reparent", node);
       // collisionBox.GetChild<CollisionShape2D>(0).SetDeferred("disabled", true);
		hitBox.SetEnabled(false);
		audioPlayer.Play();
		
        Velocity = Vector2.Zero;
		speed = 0;
		
		
	}

    public virtual void HitHurtBox()
	{
		
        //CallDeferred("reparent", node);
        //collisionBox.GetChild<CollisionShape2D>(0).Set("disabled", true);

        Velocity = Vector2.Zero;
        speed = 0;
		
		QueueFree();
    }

	void Destroy()
	{
		QueueFree();
	}
	

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
	{
		var velo = shootDirection * speed;
		if (fallDown)
		{
			Rotation = Mathf.DegToRad(-90);
            velo.Y += 120;
		}

		Velocity = velo;
        MoveAndSlide();


    }
	
}
