using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[Icon("res://Assets/Locks and Doors/Icons/DoorIcon.png")]
public partial class Door : Node2D, IDoor
{
    public List<ILock> lockList { get; set; } = new();
    public bool opened { get; set; } = false;
    [Export] public IDoor.GateType type { get; set; }

    public override void _EnterTree()
    {
        
        base._Ready();
        try
        {
            foreach(ILock locke in GetChildren().OfType<ILock>())
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

    public override void _Ready()
    {
        base._Ready();
        AttemptToOpen();
        /*if(GetParentOrNull<Door>() != null)
        {
            type = GateType.FORCED;
        }*/
    }

    //rename to update door state

    public virtual bool AttemptToOpen()
    {

        opened = false;
        int locksUnlocked = 0;
        foreach (ILock locke in lockList)
        {
            if (locke.unlocked)
            {
                locksUnlocked++;
            }
        }

        switch (type)
        {
            case IDoor.GateType.AND:
                if (locksUnlocked == lockList.Count())
                {
                    opened = true;
                }
                break;
            
            case IDoor.GateType.XAND:
                if (locksUnlocked > 0 && locksUnlocked == lockList.Count())
                {
                    opened = true;
                }
                break;

            case IDoor.GateType.NAND:
                if (locksUnlocked < lockList.Count())
                {
                    opened = true;
                }
                break;

            case IDoor.GateType.OR:
                if (locksUnlocked > 0)
                {
                    opened = true;
                }
                break;
            
            case IDoor.GateType.NOR:
                if (locksUnlocked == 0)
                {
                    opened = true;
                }
                break;

            case IDoor.GateType.XOR:
                if (locksUnlocked > 0 && locksUnlocked < lockList.Count())
                {
                    opened = true;
                }
                break;

            case IDoor.GateType.XNOR:
                if (locksUnlocked == 0 || locksUnlocked == lockList.Count())
                {
                    opened = true;
                }
                break;

            case IDoor.GateType.SAVEONE:
                if (locksUnlocked == lockList.Count()-1)
                {
                    opened = true;
                }
                break;
            
            case IDoor.GateType.ODDS:
                if (locksUnlocked % 2 == 1)
                {
                    opened = true;
                }
                break;
            
            case IDoor.GateType.EVENS:
                if (locksUnlocked % 2 == 0)
                {
                    opened = true;
                }
                break;
            
            case IDoor.GateType.FORCED:
                opened = true;
                break;

            default:
                opened = false;
                throw new Exception("Default case in " + this.Name + " | " + type);

        }

        

        if (opened)
        {
            return true;
        }

        //attempt to close()
        Close();
        return false;
    }

    

    public virtual bool Close()
    {
        opened = false;
       
        return true;
    }

   
    
    
}
