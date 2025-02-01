using Godot;
using System;

public abstract partial class TexturePanel_YesNoDialog : TextureRect
{

    public TextureButton OpenDialogButton;
    protected TextureButton YesButton, NoButton;
    bool Shown = false;
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
    }
    protected abstract void YesPressed();
}
