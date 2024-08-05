using Godot;
using System;

public partial class CameraSmoother : Camera2D
{
    [Export] NodePath TargetNodepath = null;
    Node2D targetNode;
    [Export] public float lerpSpeedX = 0.05f;
    [Export] public float lerpSpeedY = 0.05f;

    


    public override void _Ready()
    {

        targetNode = GetNode<Node2D>(TargetNodepath);
    }

    public override void _Process(double delta)
    {
        try
        {


            Vector2 cameraPosition;
            cameraPosition.X = Mathf.Lerp(GlobalPosition.X, targetNode.GlobalPosition.X, lerpSpeedX);

            cameraPosition.Y = Mathf.Lerp(GlobalPosition.Y, targetNode.GlobalPosition.Y, lerpSpeedY);



        
            GlobalPosition = cameraPosition;
        }
        catch 
        {

        }
    }


}
