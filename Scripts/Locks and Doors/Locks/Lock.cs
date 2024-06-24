using Godot;
using System;

public partial class Lock : Area2D
{
    public bool unlocked;
    protected Door door;
	protected AnimatedSprite2D sprite;
    protected AudioStreamPlayer2D audioPlayer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        audioPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
		AreaEntered += (node) => UnlockMe();
	}

    public override void _EnterTree()
    {
        base._Ready();
        door = GetParent<Door>();
        door.lockList.Add(this);
    }

    protected virtual void UnlockMe()
    {
        if(unlocked == true)
        {
            return ;
        }
        unlocked = true;
        door.Open();
    }
    
    protected virtual void LockMe()
    {
        if(unlocked == false)
        {
            return ;
        }
        unlocked = false;
        door.Close();
    }
    
}
