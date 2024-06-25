using Godot;
using System;
using System.Collections.Generic;

public partial class Door : Node2D
{
    public List<Lock> lockList = new();
    public bool opened = false;
   

    public virtual bool AttemptToOpen()
    {
        GD.Print("Attempting to Open " + Name);

        foreach (Lock locke in lockList)
        {
            if (!locke.unlocked)
            {
                GD.Print(locke.Name + " is LOCKED; Open failed.");
                opened = false;
                return false ;
            }
        }
        GD.Print("Open succeeded " + Name);
        opened = true;
        return true ;
    }
    
    public virtual bool AttemptToClose()
    {
		
        opened = false;
        return true;
    }

   
    
    
}
