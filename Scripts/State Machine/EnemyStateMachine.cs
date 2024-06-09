using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;


public partial class EnemyStateMachine : StateMachine
{
    public player link;
    public EnemyV2 body;

    // Called when the node enters the scene tree for the first time.
    public  void SetUp()
	{
        link = (player)GetTree().GetFirstNodeInGroup("Player");

		body = Owner.GetNode<EnemyV2>(Owner.GetPath()+"/EnemyLogic");
		foreach (EnemyState enemyState in States)
		{
            enemyState.SetUp(new Dictionary<string, object>() {
                {"Machine", this },
                {"Animator", body.animator },
                {"StatusAnimator", body.statusAnimator },
                {"Body", body } });
			
		}
		ChangeState("EnemyIdleState", null);
        

        GD.Print(Owner.Name + " is " + state.Name);

    }



    
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        

           /* foreach (Node2D node in body.fov.GetOverlappingBodies())
            {
                if (node == link)
                {
                    if (state == States[0]|| state == States[3]|| state == States[5])
                        ChangeState("EnemyChaseState", null);
                    body.hasTarget = true;
                }
            }*/
        
    }
}
