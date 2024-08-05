using Godot;
using System;

public partial class HurtBox2D : Area2D
{

    [Export] public uint layers;
    CollisionShape2D collisionShape;

    public override void _Ready()
    {
        CollisionLayer = 0;
        CollisionMask = layers;
        AreaEntered += (AreaEntered) => OnAreaEntered(AreaEntered);
        collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
    }

    
   public void SetEnabled(bool enabled)
    {
        collisionShape.SetDeferred("disabled", !enabled);
    }



    public void OnAreaEntered(Area2D hitBox)
    {
        if (hitBox.Owner.Name != Owner.Name)
        {
            GD.Print("Hurtbox Hit on " + Owner.Name);
            HitBox2D box = (HitBox2D)hitBox;
            if (box != null)
                Owner.Call("TakeDamage", box.damage, box);


        }

        
    }
}
