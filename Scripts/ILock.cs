using Godot;
using System;

public interface ILock
{
    public bool unlocked { get; set; }
    public IDoor door { get; set; }

    public CollisionShape2D shape { get; set; }

    public void UnlockMe(Node2D node);


    public void LockMe();

    public void SetUp();
    public void SetActive(bool active);
    
}
