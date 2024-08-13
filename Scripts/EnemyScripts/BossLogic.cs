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

	Area2D arrowDetector;
	CollisionShape2D arrowBlocker;
	CollisionShape2D bodyArrowBlocker;

	[Export] NodePath signalLockPath;
	SignalLock signalLock;

	[Export] float wakeUpTime = 1f;

	[Export] int bossFloorHeight = -207;

	[Export] int wakeUpHeight = 112;
	[Export] int maxLeft;
	[Export] int maxRight;
	[Export] float minFlapTime = 2f;
	[Export] float maxFlapTime = 7f;

	[Export] AudioStream bonkSound, blinkSound, landSound, flapSound, deadSound, hitSound, finalHitSound;

	bool queueTimer = false;
	int driftDirection;

	bool wakeUpTimerPlaying = false;

	bool queueDeath = false;

	[Export] float speed = 6;
	[Export] float driftSpeed = 10;

	bool finalPhase = false;
	[Export]
	int hitsToFinalTakeDown = 7;
	int bodyHits = 0;

	bool downed;
	bool flappable = false;
	public enum BodyState { Healthy, Targetable }
	BodyState state = BodyState.Healthy;
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	PackedScene deathPuppet = GD.Load<PackedScene>("res://Scenes/Enemies/Boss/boss_death_puppet.tscn");

	Area2D activationZone;
	bool active = false;

	enum FlyPosition { Left, Right, Center };
	FlyPosition flyPosition;

	BossWing activeWing;
	AudioPlayer audioPlayer;
	AudioPlayer blinkAudioPlayer;


	bool secondPhase = false;

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
		activationZone = GetNode<Area2D>("../ActivationZone");
		arrowBlocker = GetNode<CollisionShape2D>("ArrowBlocker/CollisionShape2D");
		bodyArrowBlocker = GetNode<CollisionShape2D>("ArrowBlocker/CollisionShape2D2");
		arrowDetector = GetNode<Area2D>("ArrowDetectorArea");
		audioPlayer = GetNode<AudioPlayer>("AudioStreamPlayer2D");
		blinkAudioPlayer = GetNode<AudioPlayer>("Blinker");
        //flapTimer.Timeout += Flap;

        arrowDetector.BodyEntered += (node) => BlockArrow(true);
		arrowDetector.BodyExited += (node) => BlockArrow(false);

		wakeUpTimer.Timeout += WakeUp;

		animator.AnimationFinished += AnimationFinished;
		hurtBox.SetEnabled(false);
		finalHurtBox.SetEnabled(false);

		

        foreach (BossWing wing in wings)
        {
            wing.ShowBossEyes(false);
        }

        signalLock = GetNode<SignalLock>(signalLockPath);

		wingCount = wings.Length;
		originalWingCount = wingCount;

		driftDirection = Mathf.Sign(GD.RandRange(-1, 1));

		activationZone.BodyEntered += ActivateBoss;
		hitsToFinalTakeDown--;
	}

	void BlockArrow(bool block)
	{
		if (active)
		{
			arrowBlocker.SetDeferred("disabled", !block);
		}
	}

	void Flap()
	{


		flapTimer.WaitTime = GD.RandRange(minFlapTime, maxFlapTime);
		flapTimer.Start();
		if (downed)
			return;
		foreach (BossWing wing in wings)
		{
			wing.Flap();
		}

		if(finalPhase)
		{
			bodyArrowBlocker.SetDeferred("disabled", false);
		}

		audioPlayer.PlaySound(flapSound);
	}

	void AnimationFinished()
	{
		if (animator.Animation == "HorizontalDamage")
		{
			if (downed)
			{
				animator.Play("Downed" + state.ToString());
				if (finalPhase)
				{
					bodyArrowBlocker.SetDeferred("disabled", true);
				}
			}
		}
		else if (animator.Animation == "Damage")
		{

			//GD.PrintErr("NotToThirdPhaseYet");

			animator.Play(state.ToString());
			if (bodyHits > hitsToFinalTakeDown)
			{
				animator.Play("Downed" + state.ToString());
				LoseWing(BodyState.Targetable);
				finalHurtBox.SetEnabled(false);
                audioPlayer.PlaySound(finalHitSound);

            }

        }
		else if (animator.Animation == "WakeUp" + state.ToString())
		{
            foreach (BossWing wing in wings)
            {
                wing.AnimateTakedown(true);
			}
            animator.PlayBackwards("Hit" + state.ToString());

            hitBox.SetEnabled(true);

			CheckForDeath();

			downed = false;

			//need to pick a wing
			if (finalPhase)
			{
				if(bodyHits > Mathf.CeilToInt(hitsToFinalTakeDown * 0.5f))
					bodyHits = Mathf.CeilToInt(hitsToFinalTakeDown * 0.5f);
			}
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
				if (wingCount < originalWingCount && !secondPhase)
				{
					flapTimer.Timeout += Flap;
					secondPhase = true;
				}
			}
			else
			{
				foreach (BossWing wing in wings)
				{
					if (!wing.permaDead)
					{
						wing.active = false;
						wing.Close();
					}
				}
			}
            wakeUpTimer.WaitTime = wakeUpTime;

        }
        else if (animator.Animation == "Hit" + state.ToString() && !downed)
		{
            if (!active)
            {
                active = true;
                flapTimer.Timeout += Flap;

            }
            if (PickActiveWing())
            {
                state = BodyState.Healthy;
            }
            else
            {
                EnterFinalPhase();
            }
            animator.Play(state.ToString());
			Flap();
			
			flappable = true;
		}
	}

	bool playedLandSound = false;
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if(!active) return;

		//GD.Print(flapTimer.TimeLeft);
		//body hits starts halfway


		Vector2 velocity = Velocity;
		if (!flappable)
		{

			if (!IsOnFloor())
			{
				velocity.Y += gravity * (float)delta;
				playedLandSound = false;
			}

			if (IsOnFloor())
			{
				velocity.X = (float)Mathf.Lerp(Velocity.X, 0, 0.20);
				if(!playedLandSound)
				{
					audioPlayer.PlaySound(landSound);
					playedLandSound = true;
				}

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
			velocity.Y = -speed * originalWingCount - wingCount*2;

			if (Position.Y > bossFloorHeight - wakeUpHeight)
			{


				velocity.Y *= gravity * 2 * (float)delta;
			}

			/*if(Position.Y > bossFloorHeight)
			{
				Flap();

			}*/

			else
			{

				if (Position.X < maxLeft)
				{
					driftDirection = 1;
				}
				else if (Position.X > maxRight)
				{
					driftDirection = -1;
				}



				if (Position.X < maxLeft-1 || Position.X > maxRight+1)
				{
					driftDirection *= 4;
					//Flap();
				}

				velocity.X = driftSpeed * driftDirection;


				if (wingCount < wings.Count() * 0.5f)
				{
					velocity *= 1.5f;
				}
				else if (wingCount <= 0)
				{
					velocity *= 2f;
				}
					
			}
		}
		Velocity = velocity;

		MoveAndSlide();
	}

	public void LoseWing(BodyState stateToShow = BodyState.Targetable, BossWing lostWing = null)
	{

		foreach (BossWing wing in wings)
		{
			wing.AnimateTakedown();
			wing.ShowBossEyes(false);
		}
		if (lostWing != null)
			lostWing.state = BossWing.WingState.Dead;

		//GD.Print(state.ToString());
		state = stateToShow;
		animator.Play("Hit" + state.ToString());// Targetable");
		queueTimer = true;
		hitBox.SetEnabled(false);
		hurtBox.SetEnabled(true);
		downed = true;
		flappable = false;
	}

	void HandleKnockback(HitBox2D caller)
	{
		var direction = Mathf.Sign(Position.X - caller.Position.X);

		animator.Play("HorizontalDamage");
        audioPlayer.PlaySound(hitSound);


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
            if (bodyHits <= hitsToFinalTakeDown)
				audioPlayer.PlaySound(hitSound);
        }
    } 
	public void HeadBonk()
	{
		audioPlayer.PlaySound(bonkSound);
		wakeUpTimer.WaitTime = 0.001f;
		state = BodyState.Healthy;
		LoseWing(BodyState.Healthy);
	}
			
	void FinalDeath()
	{
        BossDeathPuppet puppet = deathPuppet.Instantiate<BossDeathPuppet>();
        puppet.GlobalPosition = GlobalPosition;
        GetTree().Root.AddChild(puppet);
		signalLock.Unlock();
		audioPlayer.PlaySound(deadSound);
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

		

		//GD.Print(wingCount + " / " + originalWingCount);

        driftDirection = -MathF.Sign(Position.X);
		hurtBox.SetEnabled(false);
		
		animator.Play("WakeUp" + state.ToString());
		blinkAudioPlayer.Play();

		bool detected = deathZoneDetector.HasOverlappingAreas();
		
	
		

        
        
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
			
			activeWing = aliveWings[rand];
			if (activeWing != null)
			{
				if ((activeWing.GlobalPosition.X - GlobalPosition.X)>0)
				{
					flyPosition = FlyPosition.Left;
				}
				else if ((activeWing.GlobalPosition.X - GlobalPosition.X)<0)
				{
					flyPosition = FlyPosition.Right;
				}
			}
			

			return true;
		}
		else
			{
					flyPosition = FlyPosition.Center;
			}
		return false;
	}

	void EnterFinalPhase()
	{
        finalPhase = true;
        finalHurtBox.SetEnabled(true);
		state = BodyState.Targetable;
		bodyArrowBlocker.SetDeferred("disabled", true);
		activeWing.FlapEnded += EndFlap;
    }

	void EndFlap()
	{
		if (finalPhase)
		{
			bodyArrowBlocker.SetDeferred("disabled", true);
		}
	}

	void ActivateBoss(Node2D node)
	{
		if (!active)
		{
			active = true;
			WakeUp();
			//animator.Play("WakeUp" + state.ToString());
            /*animator.Play("Hit" + state.ToString(), -1, true);
            foreach (BossWing wing in wings)
            {
                string name = wing.Name;
                wing.AnimateTakedown(true);



            }*/
        }
    }

}
