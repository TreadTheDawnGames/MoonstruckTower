using Godot;
using System;

[Icon("res://Assets/Locks and Doors/Icons/LockIcon.png")]
public partial class Lock : Area2D, ILock
{
    [Export]
    public bool unlocked { get; set; } = false;
    public IDoor door { get; set; }
    public CollisionShape2D shape { get; set; }
	// Called when the node enters the scene tree for the first time.
	public virtual void SetUp()
	{
		
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
        try
        {
            door.AttemptToOpen();
        }
        catch
        {
            GD.PrintErr(Name + " does not have an assigned door");
        }
    }
    
    public virtual void LockMe()
    {
        
        if (unlocked == false)
        {
            return ;
        }
        unlocked = false;
        try
        {
            door.AttemptToOpen();
        }
        catch
        {
            GD.PrintErr(Name + " does not have an assigned door");
        }
    }
    
    public void SetActive(bool active)
    {
        shape.SetDeferred("disabled", !active);
    }

}
