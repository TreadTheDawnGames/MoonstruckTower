using Godot;
using System;
using System.Collections.Generic;

public partial class Door : Node2D
{
    public List<Lock> lockList = new();
    public bool opened = false;
   

    public virtual bool Open()
    {
        foreach (Lock locke in lockList)
        {
            if (!locke.unlocked)
            {
                opened = false;
                return false ;
            }
        }
        opened = true;
        return true ;
    }
    
    public virtual bool Close()
    {
        foreach (Lock locke in lockList)
        {
            if (locke.unlocked)
            {
                opened = true;
                return false;
            }
        }
        opened = false;
        return true;
    }

   
    
    
}
