using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;


public partial class EnemyStateMachine : StateMachine
{
    public player link;
    public Enemy body;

    // Called when the node enters the scene tree for the first time.
    public  void Setup()
	{
        link = (player)GetTree().GetFirstNodeInGroup("Player");

		body = Owner.GetNode<Enemy>(Owner.GetPath());
		foreach (EnemyState enemyState in States)
		{
            enemyState.SetUp(new Dictionary<string, object>() {
                {"Machine", this },
                {"Animator", Owner.GetNode<AnimatedSprite2D>(Owner.GetPath() + "/Animator") },
                {"StatusAnimator", body.statusAnimator },
                {"Body", body } });
			
		}
		ChangeState("EnemyIdleState", null);
        

        GD.Print(Owner.Name + " is " + state.Name);

    }



    
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        

            foreach (Node2D node in body.fov.GetOverlappingBodies())
            {
                if (node == link)
                {
                    if (state == States[0]|| state == States[3]|| state == States[5])
                        ChangeState("EnemyChaseState", null);
                    body.hasTarget = true;
                }
            }
        
    }
}
