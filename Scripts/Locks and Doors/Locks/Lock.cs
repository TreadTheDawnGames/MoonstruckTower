using Godot;
using System;

public partial class Lock : Area2D
{
    [Export]
    public bool unlocked = false;
    protected Door door;
	protected AnimatedSprite2D sprite;
    protected CollisionShape2D shape;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		AreaEntered += (node) => UnlockMe(node);
        shape = GetNode<CollisionShape2D>("CollisionShape2D");
	}

    public override void _EnterTree()
    {
        base._Ready();
        try
        {

        door = (Door)Owner;
        door.lockList.Add(this);
        }
        catch
        {
            GD.PrintErr(Name + " is not assigned to a door");
        }
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
    
    public virtual void LockMe()
    {
        
        if (unlocked == false)
        {
            return ;
        }
        unlocked = false;
        door.AttemptToClose();
    }
    
    public void SetActive(bool active)
    {
        shape.SetDeferred("disabled", !active);
    }

}
