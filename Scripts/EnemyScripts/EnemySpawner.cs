using Godot;
using System;

[Icon("res://Assets/Locks and Doors/Icons/SpawnerIcon.png")]
public partial class EnemySpawner : Node2D, ILock
{
	Timer respawnTimer;
	PackedScene enemyScene;
	[Export] float respawnTime = 300f;
	[Export] public int tilesToFallBeforeDeath = 3;
	AnimatedSprite2D sprite;
	enum Enemy { Moblin, Octorok}
	[Export] Enemy enemy;


    public bool unlocked { get; set; }
    public IDoor door { get; set; }
    public CollisionShape2D shape { get; set; }

	// Called when the node enters the scene tree for the first time.
	public void SetUp() {
	}

    public override void _Ready()
	{
		

		string path = "";
		switch (enemy)
		{
			case Enemy.Moblin:
				path = "res://Scenes/Enemies/dino_v3.tscn";
                    break;
			case Enemy.Octorok:
				path = "res://Scenes/Enemies/shooty_plant.tscn";
                    break;
		}

				enemyScene = GD.Load<PackedScene>(path);

		

		respawnTimer = GetNode<Timer>("Timer");
		sprite = GetNode<AnimatedSprite2D>("Sprite2D");
		respawnTimer.Timeout += RespawnEnemy;
		respawnTimer.WaitTime = respawnTime;
		RespawnEnemy();


	}

	public void StartRespawnTimer()
	{
		respawnTimer.Start();
        sprite.Play("Spawning");

    }

    void RespawnEnemy()
	{
        sprite.Play("Off");


        EnemyBase enemy = enemyScene.Instantiate<EnemyBase>();
		enemy.spawner = this;
		AddChild(enemy);

		LockMe();
		
		//enemy.GlobalPosition = GlobalPosition;
	}

    public virtual void UnlockMe(Node2D node)
    {



        if (unlocked == true)
        {
            return;
        }
        unlocked = true;
        door?.AttemptToOpen();
    }

    public virtual void LockMe()
    {

        if (unlocked == false)
        {
            return;
        }
        unlocked = false;
        door?.AttemptToOpen();
    }

    public void SetActive(bool active)
    {
        throw new NotImplementedException();
    }
}
