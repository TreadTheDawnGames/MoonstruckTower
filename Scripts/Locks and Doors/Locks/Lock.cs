using Godot;
using System;

public partial class Lock : Area2D, ILock
{
    [Export]
    public bool unlocked { get; set; } = false;
    public IDoor door { get; set; }
    public CollisionShape2D shape { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		//sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		AreaEntered += (node) => UnlockMe(node);
        shape = GetNode<CollisionShape2D>("CollisionShape2D");
	}

    
    

    public virtual void UnlockMe(Node2D node)
    {

            
        
        if(unlocked == true)
        {
            return ;
        }
        unlocked = true;
        door.AttemptToOpen();
    }
    
    public virtual void LockMe()
    {
        
        if (unlocked == false)
        {
            return ;
        }
        unlocked = false;
        door.AttemptToOpen();
    }
    
    public void SetActive(bool active)
    {
        shape.SetDeferred("disabled", !active);
    }

}
