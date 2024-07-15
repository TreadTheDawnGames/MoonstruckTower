using Godot;
using System;

public partial class ZeldaLogo : AnimatedSprite2D
{
    Timer timer;
    public override void _Ready()
    {
        timer = GetNode<Timer>("Timer");
        timer.Timeout += StartNewAnimation;

        double rand = GD.RandRange(5, 12);
        timer.WaitTime = rand;
        timer.Start();
    }

    void StartNewAnimation()
    {
        Play();
        double rand = GD.RandRange(5,12);
        timer.WaitTime = rand;
        timer.Start();
    }

    
}
