using Godot;
using System;
using System.Collections.Generic;
using static Godot.TextServer;

public partial class EnemyState : State
{
	public EnemyStateMachine machine;
	public AnimatedSprite2D animator;
	public AnimatedSprite2D statusAnimator;
	public EnemyBase logic;
    public int moveDirection = 0;
    public float speedMultiplier = 1;
    public bool jumping = false;
    protected bool isCurrentState { get; private set; } = false;
    public override void SetUp(Dictionary<string, object> message)
    {
        base.SetUp(message);
        machine = (EnemyStateMachine)message["Machine"];
        animator = (AnimatedSprite2D)message["Animator"];
        statusAnimator = (AnimatedSprite2D)message["StatusAnimator"];
        logic = (EnemyBase)message["Body"];
    }
    public override void OnStart(Dictionary<string, object> message)
    {
        base.OnStart(message);
        isCurrentState = true;
    }
    public override void OnExit(string nextState)
    {
        base.OnExit(nextState);
        isCurrentState = false;
    }

}

    



