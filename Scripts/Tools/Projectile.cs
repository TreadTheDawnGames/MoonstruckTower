using Godot;
using System;
using System.Linq;

public partial class Projectile : CharacterBody2D
{
	[Export] public float speed = 300;
	[Export] public Vector2 shootDirection;
	[Export] bool destroyOnContact = false;
	Area2D collisionBox;
    HitBox2D hitBox;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		collisionBox = GetNode<Area2D>("CollisionBox");
		hitBox = GetNode<HitBox2D>("HitBox2D");

		collisionBox.BodyEntered += (node) => HitWorld(node);

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
        collisionBox.GetChild<CollisionShape2D>(0).SetDeferred("disabled", true);
		hitBox.SetEnabled(false);

        Velocity = Vector2.Zero;
		speed = 0;
		
		
	}

    public virtual void HitHurtBox()
	{
		
		GD.Print("Called HitHurtBox()");
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
    public override void _Process(double delta)
	{
		Velocity = shootDirection*speed;

        

        MoveAndSlide();
		

	}

}
