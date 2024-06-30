using Godot;
using System;

public partial class Lock : Area2D
{
    [Export]
    public bool unlocked = false;
    protected Door door;
	protected AnimatedSprite2D sprite;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		AreaEntered += (node) => UnlockMe(node);
	}

    public override void _EnterTree()
    {
        base._Ready();
        door = GetParent<Door>();
        door.lockList.Add(this);
    }

    protected virtual void UnlockMe(Node2D node)
    {

            
        
        if(unlocked == true)
        {
            return ;
        }
        unlocked = true;
        door.AttemptToOpen();
    }
    
    protected virtual void LockMe()
    {
        
        if (unlocked == false)
        {
            return ;
        }
        unlocked = false;
        door.AttemptToClose();
    }
    
}
