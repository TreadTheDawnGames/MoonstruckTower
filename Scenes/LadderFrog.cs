using Godot;
using System;
using System.Linq;

public partial class LadderFrog : Node2D
{
    AnimatedSprite2D animator;
    bool deactivated = true;
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    int direction = 0;

    PackedScene ladderScene = GD.Load<PackedScene>("res://Scenes/Tools/Ladder/tool_ladder.tscn");
    public override void _Ready()
    {
        base._Ready();
        animator = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animator.AnimationFinished += AnimationEnd;
    }

    void AnimationEnd()
    {
        if (animator.Animation == "Activate")
        {
            
            animator.Play("SpitLadder");
        }
        else if (animator.Animation == "SpitLadder")
        {
            animator.Play("Deactivate");
            GD.Print("Ladder Spat");
            
            
            
            var ladder = ladderScene.Instantiate<RigidBody2D>();
            float jumpInPixels = -Mathf.Sqrt(2 * gravity * 16);

            //var ladder = (Ladder)ladderPhysics.GetScript();
            ladder.GlobalPosition = GlobalPosition;
            ladder.Rotation = Rotation;

            float hPower = 0;
            if(Rotation == Mathf.DegToRad(0))
            {
                hPower = 32 * direction;
            }
            ladder.LinearVelocity = new Vector2(hPower, jumpInPixels);
            

            GetTree().Root.GetNode("Game").AddChild(ladder);
            ladder.AddToGroup("Ladders");


        }
        else if(animator.Animation == "Deactivate")
        {
            animator.Play("Idle");
            deactivated = true;
        }
    }

    void TakeDamage(int damage, HitBox2D box)
	{
        if(deactivated)
        {
            direction = Mathf.Sign(GlobalPosition.X - box.GlobalPosition.X);
            foreach (Ladder existingLadder in GetTree().GetNodesInGroup("Ladders").OfType<Ladder>())
            {
                existingLadder.Despawn(true);
            }

            if (box.Owner is Projectile)
            {
                //get arrow owner script /detect if box was on projectile and if it was, delete it

                Projectile arrow = box.Owner.GetNode<Projectile>(box.Owner.GetPath());

                arrow.HitHurtBox();

            }
            Player link = (Player)GetTree().GetFirstNodeInGroup("Player");  // (Player)hitBox.Owner;

                link.toolBag.GetNode<LadderSpawner>("LadderSpawner").ladderPlaced = true;
            

            animator.Play("Activate");
            deactivated = false;
        }
	}
}
