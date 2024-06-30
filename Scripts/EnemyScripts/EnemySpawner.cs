using Godot;
using System;

public partial class EnemySpawner : Node2D
{
	Timer respawnTimer;
	[Export]
	PackedScene enemyScene;
	[Export] float respawnTime = 300f;
	AnimatedSprite2D sprite;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
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
		
		//enemy.GlobalPosition = GlobalPosition;
		AddChild(enemy);
	}
}
