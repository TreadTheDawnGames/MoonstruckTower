using Godot;
using System;
using System.Diagnostics.Metrics;

public abstract partial class TexturePanel_YesNoDialog : TextureRect
{

    public TextureButton OpenDialogButton;
    protected TextureButton YesButton, NoButton;
    public static bool Shown { get; private set; } = false;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //OpenDialogButton = GetParent<TextureButton>();
        /* GetParent().CallDeferred("remove_child", this);
         GetTree().Root.CallDeferred("add_child", this);
         GetTree().Root.CallDeferred("move_child", this, GetTree().Root.GetChildCount()-1);
        */
        Position = Vector2.Zero;
        GlobalPosition = new Vector2((GetViewportRect().Size.X/2) - Texture.GetWidth()/2, (GetViewportRect().Size.Y/2) - Texture.GetHeight()/2);
        
        ZIndex = 1000;

        YesButton = GetNode<TextureButton>("Yes");
        NoButton = GetNode<TextureButton>("No");
        Hide();
        OpenDialogButton.Pressed += () => {
            if (!Shown) MyShow(); else MyHide();
            OpenDialogButton.Disabled = true;
        };

        YesButton.Pressed += YesPressed;

        NoButton.Pressed += NoPressed;
        NoButton.GrabFocus();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (@event.IsActionPressed("ui_back"))
        {
            NoPressed();
            GetViewport().SetInputAsHandled();
        }
    }


    protected void MyShow()
    {
        Show();
        Shown = true;
    }

    protected void MyHide()
    {
        Hide();
        Shown = false;
    }

    protected virtual void NoPressed() {
        MyHide();
        OpenDialogButton.Disabled = false;
        GetTree().CreateTimer(0.1).Timeout += QueueFree;
        OpenDialogButton.GrabFocus();
    }
    protected virtual void YesPressed()
    {
        OpenDialogButton.GrabFocus();
    }
}
