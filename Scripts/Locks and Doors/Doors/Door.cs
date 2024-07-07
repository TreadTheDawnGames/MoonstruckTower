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
            GD.Print(ex.Message);
            GD.Print(ex.StackTrace);
            GD.PrintErr(Name + " is not assigned to a locklist");
        }
    }

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
