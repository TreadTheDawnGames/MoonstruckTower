using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Door : Node2D
{
    public List<Lock> lockList = new();
    public bool opened = false;

    public override void _EnterTree()
    {
        base._Ready();
        try
        {
            foreach(Lock locke in GetChildren().OfType<Lock>())
            {
                locke.door = this;

                lockList.Add(locke);
            }
        }
        catch(Exception ex) 
        {
            GD.PrintErr(Name + " is not assigned to a locklist");
            GD.Print(ex.Message);
            GD.Print(ex.StackTrace);
        }
    }

    public virtual bool AttemptToOpen()
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
    
    public virtual bool AttemptToClose()
    {
		
        opened = false;
        return true;
    }

   
    
    
}
