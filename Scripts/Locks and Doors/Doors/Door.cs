using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Door : Node2D, IDoor
{
    public List<ILock> lockList { get; set; } = new();
    public bool opened { get; set; } = false;

    enum GateType { AND, NAND, OR, NOR, XOR, XNOR };
    [Export] GateType type = GateType.AND;

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
        GD.Print(locksUnlocked);

        switch (type)
        {
            case GateType.AND:
                if (locksUnlocked == lockList.Count())
                {
                    opened = true;
                }
                break;

            case GateType.NAND:
                if (locksUnlocked < lockList.Count())
                {
                    opened = true;
                }
                break;

            case GateType.OR:
                if (locksUnlocked > 0)
                {
                    opened = true;
                }
                break;
            
            case GateType.NOR:
                if (locksUnlocked == 0)
                {
                    opened = true;
                }
                break;

            case GateType.XOR:
                if (locksUnlocked > 0 && locksUnlocked < lockList.Count())
                {
                    opened = true;
                }
                break;

            case GateType.XNOR:
                if (locksUnlocked == 0 || locksUnlocked == lockList.Count())
                {
                    opened = true;
                }
                break;
            default:
                opened = false;
                throw new Exception("Default case in " + this.Name + " | " + type);

        }


        if (opened)
        {
            GD.Print("Opening");
            return true;
        }

        //attempt to close()
        Close();
            GD.Print("NOT Opening");
        return false;
    }

    public virtual bool Close()
    {
        opened = false;
       
        return true;
    }

   
    
    
}
