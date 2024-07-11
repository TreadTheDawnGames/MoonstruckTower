using Godot;
using System;
using System.Linq;
using System.Net.NetworkInformation;

public partial class BossWing : Door
{
	BossLogic logic;
	AnimatedSprite2D animator;
	public enum WingState { Healthy, Targetable, Dead}
	public WingState state = WingState.Healthy;

	public bool permaDead = false;

	[Export] public bool active = false;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		
		animator = GetNode<AnimatedSprite2D>("Animator");
		animator.AnimationFinished += AnimationFinished;
		logic = (BossLogic)Owner;
		animator.Play("Full"+state.ToString());
		foreach(BossOrb eye in GetChildren().OfType<BossOrb>())
		{
			eye.door = this;
		}
	}

	void AnimationFinished()
	{
		string myState = state.ToString();
		if(state.ToString().Contains("Dead"))
		{
			myState = "Dead";
		}
		if (animator.Animation == "Flap" + myState)
		{

			ShowBossEyes(true);
            animator.Play("Full"+ myState);
		}
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	

    public override bool AttemptToOpen()
    {
		if (base.AttemptToOpen())
		{
			state = WingState.Dead;
			logic.LoseWing(lostWing: this);
			ShowBossEyes(false);
			
		}
		return true;

    }

    public override bool Close()
    {
        if( base.Close())
		{
			//state = WingState.Healthy;
			foreach(BossOrb orb in lockList)
			{
				
				orb.LockMe();
			}
			state = WingState.Healthy;
			return true;
		}
		return false;
    }

    public void AnimateTakedown(bool reverse = false)
	{
        string myState = state.ToString();
        if (state.ToString().Contains("Dead"))
        {
            myState = "Dead";
        }
        animator.Play("Hit" +myState,customSpeed:reverse?-1:1,reverse);
	}

	public void ShowBossEyes(bool canSee)
	{
		foreach (Lock locke in lockList)
		{
			if (canSee)
			{
				locke.Show();
			}
			else
			{
				locke.Hide();
			}
			locke.SetActive(canSee);
		}
    }
    public void TryBecomeTarget()
	{
        
            state = WingState.Targetable;
			foreach(BossOrb locke in lockList)
			{
				locke.Activate();
				
			}
		//ShowBossEyes(true);
    }

	public void PermaKill()
	{
		state = WingState.Dead;
		ShowBossEyes(false);
		permaDead = true;
	}
	public void Flap()
	{
        string myState = state.ToString();
        if (state.ToString().Contains("Dead"))
        {
            myState = "Dead";
        }
        animator.Play("Flap" + myState);
		ShowBossEyes(false);
	}

	public void Activate()
	{
		active = true;
		TryBecomeTarget();
	}
}
