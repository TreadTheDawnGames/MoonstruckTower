using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;
using System.Web;

public partial class BossLogic : CharacterBody2D
{
	BossWing[] wings;
	HurtBox2D hurtBox;
	HurtBox2D finalHurtBox;
	HitBox2D hitBox;
	Area2D deathZoneDetector;

	AnimatedSprite2D animator;
	Timer flapTimer;
	Timer wakeUpTimer;
	int wingCount;
	int originalWingCount;

	[Export] int bossFloorHeight = -207;


	bool queueTimer = false;
	int driftDirection;

	bool wakeUpTimerPlaying = false;

	bool queueDeath = false;

	[Export] float speed = 6;
	[Export] float driftSpeed = 10;

	bool finalPhase = false;
	[Export]
	int hitsToFinalTakeDown = 2;//7;
	int bodyHits = 0;
	int activeWings;

	bool downed;
	bool flappable = true;
	enum BodyState { Healthy, Targetable }
	BodyState state = BodyState.Healthy;
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	PackedScene deathPuppet = GD.Load<PackedScene>("res://Scenes/Enemies/boss_death_puppet.tscn");

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animator = GetNode<AnimatedSprite2D>("Body");
		wings = GetChildren().OfType<BossWing>().ToArray();
		flapTimer = GetNode<Timer>("FlapTimer");
		wakeUpTimer = GetNode<Timer>("WakeUpTimer");
		hitBox = GetNode<HitBox2D>("HitBox2D");
		hurtBox = GetNode<HurtBox2D>("HurtBox2D");
		finalHurtBox = GetNode<HurtBox2D>("FinalHurtBox");
		deathZoneDetector = GetNode<Area2D>("DeathZoneDetector");
		flapTimer.Timeout += Flap;
		wakeUpTimer.Timeout += WakeUp;

		animator.AnimationFinished += AnimationFinished;
		hurtBox.SetEnabled(false);
		finalHurtBox.SetEnabled(false);

		/*foreach (BossWing wing in wings)
        {
            if (activeWings < 1)
            {
                wing.AttemptToClose();
                activeWings++;
            }
        }*/
		wings[0].state = BossWing.WingState.Targetable;
		wings[1].state = BossWing.WingState.Healthy;

		wingCount = wings.Length;
		originalWingCount = wingCount;

		driftDirection = Mathf.Sign(GD.RandRange(-1, 1));

		Flap();

	}

	void Flap()
	{


		flapTimer.WaitTime = GD.RandRange(2f, 7f);
		flapTimer.Start();
		if (downed)
			return;
		foreach (BossWing wing in wings)
		{
			wing.Flap();
		}
	}

	void AnimationFinished()
	{
		if (animator.Animation == "HorizontalDamage")
		{
			if (downed)
			{
				animator.Play("Downed" + state.ToString());
			}
		}
		else if (animator.Animation == "Damage")
		{

			//GD.PrintErr("NotToThirdPhaseYet");

			animator.Play(state.ToString());
			if (bodyHits > hitsToFinalTakeDown)
			{
				animator.Play("Downed" + state.ToString());
				LoseWing();
				finalHurtBox.SetEnabled(false);

			}
			
		}
		else if (animator.Animation == "WakeUp" + state.ToString())
		{
			hitBox.SetEnabled(true);

			CheckForDeath();

			downed = false;

			//need to pick a wing

			if (deathZoneDetector.HasOverlappingAreas())
			{

				foreach (BossWing wing in wings)
				{
					string wingName = wing.Name;
					if (wing.state == BossWing.WingState.Dead && !wing.permaDead)
					{
						wing.PermaKill();
						wingCount--;
					}


				}
			}
			else
			{
				foreach (BossWing wing in wings)
				{
					if (!wing.permaDead)
						wing.AttemptToClose();
				}
			}
			;


			if (PickActiveWing())
			{
				state = BodyState.Healthy;
			}
			else
			{
				EnterFinalPhase();
			}



			animator.Play("Hit" + state.ToString(), -1, true);
			foreach (BossWing wing in wings)
			{
				string name = wing.Name;
				wing.AnimateTakedown(true);



			}
		}
		else if (animator.Animation == "Hit" + state.ToString() && !downed)
		{
			animator.Play(state.ToString());
			foreach (BossWing wing in wings)
			{
				wing.Flap();

			}
			if (finalPhase)
			{
				bodyHits = (int)(hitsToFinalTakeDown * 0.5f);
			}
			flappable = true;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 velocity = Velocity;
		if (!flappable)
		{

			if (!IsOnFloor())
				velocity.Y += gravity * (float)delta;

			if (IsOnFloor())
			{
				velocity.X = (float)Mathf.Lerp(Velocity.X, 0, 0.20);


				if (queueTimer)
				{

					if (wakeUpTimer.TimeLeft == 0)
					{
						wakeUpTimer.Start();
						queueTimer = false;
					}
				}

				if (queueDeath)
				{
					FinalDeath();
				}

			}
			else
			{
				velocity.X = (float)Mathf.Lerp(Velocity.X, 0, 0.15);
			}

		}
		else
		{
			velocity.Y = -speed * originalWingCount - wingCount;

			if (GlobalPosition.Y > bossFloorHeight - 64)
			{

				Flap();

				velocity.Y *= gravity * 2 * (float)delta;
			}
			else
			{


				if (GlobalPosition.X < -31)
				{
					driftDirection = 1;
				}
				else if (GlobalPosition.X > 31)
				{
					driftDirection = -1;
				}

				if (GlobalPosition.X < -32 || GlobalPosition.X > 32)
				{
					driftDirection *= 4;
				}

				velocity.X = driftSpeed * driftDirection;
			}
		}
		Velocity = velocity;

		MoveAndSlide();
	}

	public void LoseWing(BossWing lostWing = null)
	{
		foreach (BossWing wing in wings)
		{
			wing.AnimateTakedown();
			wing.ShowBossEyes(false);
		}
		if (lostWing != null)
			lostWing.state = BossWing.WingState.Dead;

		GD.Print(state.ToString());
		state = BodyState.Targetable;
		animator.Play("Hit" + state.ToString());// Targetable");
		queueTimer = true;
		hitBox.SetEnabled(false);
		hurtBox.SetEnabled(true);
		downed = true;
		flappable = false;
	}

	void HandleKnockback(HitBox2D caller)
	{
		var direction = Mathf.Sign(GlobalPosition.X - caller.GlobalPosition.X);

		animator.Play("HorizontalDamage");
		var hitVelX = direction * 75 * 2;
		var hitVelY = GD.RandRange(6, 12);
		float jumpInPixels = -Mathf.Sqrt(2 * gravity * hitVelY);

		//logic.velocity.Y = jumpInPixels;



		Velocity = new Vector2(hitVelX, jumpInPixels);
	}

	void TakeDamage(int damage, HitBox2D caller)
	{
		if (caller.Owner is Arrow)
		{
			Arrow arrow = (Arrow)caller.Owner;
			arrow.HitHurtBox();
		}
		if (!finalPhase && downed)
		{


			HandleKnockback(caller);
		}
		else
		{
			bodyHits++;
			if (bodyHits > hitsToFinalTakeDown + 1)
			{
				HandleKnockback(caller);
				downed = true;
				return;
			}
			animator.Play("Damage");
		}
	} 
		/*if (wakeUpTimer.TimeLeft == 0)
		{
			
			
				wakeUpTimer.Start();
				wakeUpTimerPlaying = true;
				queueTimer = false;
			
			
		}*/
			
	void FinalDeath()
	{
        BossDeathPuppet puppet = deathPuppet.Instantiate<BossDeathPuppet>();
        puppet.GlobalPosition = GlobalPosition;
        GetTree().Root.AddChild(puppet);
        QueueFree();
    }

    void CheckForDeath()
	{
        if (deathZoneDetector.HasOverlappingAreas())
        {
            if (finalPhase)
            {
				queueDeath = true; ;
            }
        }
    }

	void WakeUp()
	{
		CheckForDeath();

            driftDirection = -MathF.Sign(GlobalPosition.X);
		hurtBox.SetEnabled(false);
		animator.Play("WakeUp" + state.ToString());

		bool detected = deathZoneDetector.HasOverlappingAreas();
		
	
		

        
        
        //if not in kill zone
        //wing.attemptToClose
    }

	bool PickActiveWing()
	{
		List<BossWing> aliveWings = new List<BossWing>();
		foreach(var wing in wings)
		{
			if (!wing.permaDead)
			{
				aliveWings.Add(wing);
			}
		}
		if (aliveWings.Count > 0)
		{

			int rand = (int)(GD.Randi() % (uint)aliveWings.Count);
			aliveWings[rand].Activate();
			return true;
		}
		return false;
	}

	void EnterFinalPhase()
	{
        finalPhase = true;
        finalHurtBox.SetEnabled(true);
		state = BodyState.Targetable;
    }

}
