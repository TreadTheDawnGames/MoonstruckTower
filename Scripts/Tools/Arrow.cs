using Godot;
using System;
using System.Linq;

public partial class Arrow : Projectile
{
    bool notRetaliating = true;

    protected override void HitWorld(Node2D node)
    {

        //GD.Print("HitWorld Called");

        

        if (node is PhysicsBody2D)
        {
            PhysicsBody2D body = (PhysicsBody2D)node;

            if (body.GetCollisionLayerValue(13))
            {

                ////GD.Print("Got 13");
                fallDown = false;
                shootDirection *= -0.75f;
                var scale = Scale;
                scale.X *= -1;
                Scale = scale;
                hitBox.SetEnabled(true);
                notRetaliating = false;
                hitBox.SetCollisionLayerValue(8, false);
                Modulate = new Color(1, 0, 0);

                if(body.Owner is BossLogic)
                {
                    var boss = body.Owner as BossLogic;
                    boss.HitWithArrow();
                }

                return;

            }
        }
        if (node is TileMap)
        {
            TileMap body = (TileMap)node;
            //Tilemap.get_coords_for_body_rid(KinematicCollision2D.get_collider_rid())
            if(body.Name=="MetalTileMap")
            {
                ////GD.Print("Got 13");
                fallDown = false;
                shootDirection *= -0.75f;
                var scale = Scale;
                scale.X *= -1;
                Scale = scale;
                hitBox.SetEnabled(true);
                notRetaliating = false;

                return;

            }
        }
        base.HitWorld(node);

        
    }

    public override void _PhysicsProcess(double delta)
    {
    
        var velo = shootDirection * speed;
        if (fallDown && notRetaliating)
        {
            Rotation = Mathf.DegToRad(-90);
            velo.Y += 120;
        }

        Velocity = velo;
        MoveAndSlide();


    }
}
