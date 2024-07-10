using Godot;
using System;

public partial class EnemySpawner : Node2D
{
	Timer respawnTimer;
	PackedScene enemyScene;
	[Export] float respawnTime = 300f;
	AnimatedSprite2D sprite;
	enum Enemy { Moblin, Octorok}
	[Export] Enemy enemy;

	[Export] SignalLock sLock;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		string path = "";
		switch (enemy)
		{
			case Enemy.Moblin:
				path = "res://Scenes/Enemies/moblin_v3.tscn";
                    break;
			case Enemy.Octorok:
				path = "res://Scenes/Enemies/octorok_v2.tscn";
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

		sLock?.Lock();

        EnemyBase enemy = enemyScene.Instantiate<EnemyBase>();
		enemy.spawner = this;
		enemy.sLock = sLock;
		
		//enemy.GlobalPosition = GlobalPosition;
		AddChild(enemy);
	}
}
