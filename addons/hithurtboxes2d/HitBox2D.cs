using Godot;
using System;
using System.Collections.Generic;

public partial class HitBox2D : Area2D
{
    [Export] public int damage = 1;

    CollisionShape2D[] myShape;

    public override void _Ready()
    {
        CollisionLayer = 2;
        CollisionMask = 0;

        List<CollisionShape2D> nodes = new List<CollisionShape2D>();

        foreach (CollisionShape2D child in GetChildren())
        {
                nodes.Add(child);
        }
        myShape = nodes.ToArray() ;
    }

    public void SetEnabled(bool enabled)
    {
        foreach (CollisionShape2D shape in myShape)
        {
            shape.SetDeferred("disabled", !enabled);

        }
    }

    public bool Identify()
    {
        GD.Print("I am HitBox2D");
        return true;
    }


}
