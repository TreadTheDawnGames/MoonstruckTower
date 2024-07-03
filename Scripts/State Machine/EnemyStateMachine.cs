using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;


public partial class EnemyStateMachine : StateMachine
{
    public EnemyBase body;

    // Called when the node enters the scene tree for the first time.
    public  void SetUp()
	{

		body = Owner.GetNode<EnemyBase>(Owner.GetPath());
		foreach (EnemyState enemyState in States)
		{
            enemyState.SetUp(new Dictionary<string, object>() {
                {"Machine", this },
                {"Animator", body.animator },
                {"StatusAnimator", body.statusAnimator },
                {"Body", body } });
			
		}
		ChangeState("EnemyIdleState", null);
        


    }
}
