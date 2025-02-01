using Godot;
using System;

public partial class Fader : ColorRect
{
	AnimationPlayer animator;
    
    [Signal]
    public delegate void FadedInEventHandler();
    
    [Signal]
    public delegate void FadedOutEventHandler();

    public override void _EnterTree()
    {
        base._Ready();
        animator = GetNode<AnimationPlayer>("AnimationPlayer");
        animator.AnimationFinished += (finishedAnimation) => AnimationFinished(finishedAnimation);
        Show();
        //FadeIn();
    }

    public void PlayIdle()
    {
        animator.Play("Idle");
    }

   

    void AnimationFinished(string finishedAnimation)
    {
        if(finishedAnimation == "FadeIn")
        {
            EmitSignal(SignalName.FadedIn);

        }
        else if(finishedAnimation == "FadeOut")
        {
            EmitSignal(SignalName.FadedOut);
        }
    }


    public void FadeIn()
    {
        GD.Print("Fading in");
        animator.Play("FadeIn");
        
    }
    public void FadeOut()
    {
        animator.Play("FadeOut");
    }



}
