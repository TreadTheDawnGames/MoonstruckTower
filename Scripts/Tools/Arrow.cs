using Godot;
using System;
using System.Linq;

public partial class Arrow : Projectile
{
    protected override void HitWorld(Node2D node)
    {
        base.HitWorld(node);
        var areas = node.GetChildren().OfType<CharacterBody2D>();



        if (node is CharacterBody2D)
        {
            CharacterBody2D body = (CharacterBody2D)node;

            if (body.GetCollisionLayerValue(13))
            {
                GD.Print("Got 13");
                fallDown = true;
                hitBox.SetEnabled(true);

            }
        }

    }
}
