using Godot;
using System;

public partial class HurtBox2D : Area2D
{

    [Export] public uint layers;

    public override void _Ready()
    {
        CollisionLayer = 0;
        CollisionMask = layers;
        AreaEntered += (AreaEntered) => OnAreaEntered(AreaEntered);
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
