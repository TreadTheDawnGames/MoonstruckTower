using Godot;
using System;

public partial class LadderFrog : Node2D
{
    AnimatedSprite2D animator;

    public override void _Ready()
    {
        base._Ready();
        animator = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }
    void TakeDamage(int damage, HitBox2D hitBox)
	{

	}
}
