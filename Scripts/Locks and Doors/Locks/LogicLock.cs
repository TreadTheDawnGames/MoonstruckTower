using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class LogicLock : Lock, IDoor
{
	enum GateType { AND, NAND, OR, NOR, XOR, XNOR };
	[Export] GateType type = GateType.AND;

   

    public List<ILock> lockList { get; set; }
    public bool opened { get; set; }
    
    


    public override void _EnterTree()
    {
        try
        {
            
            
               

                foreach (ILock locke in GetChildren().OfType<ILock>())
                {
                    locke.door = this;
                    lockList.Add(locke);
                }

        }
        catch (Exception ex)
        {
            GD.PrintErr(Name + " is not assigned to a locklist");
            GD.Print(ex.Message);
            GD.Print(ex.StackTrace);
        }
    }

    public bool AttemptToOpen()
    {
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
            case GateType.AND:
                if(locksUnlocked == lockList.Count())
                {
                    unlocked = true;
                }
                break;
                
            case GateType. NAND:
                if(locksUnlocked < lockList.Count())
                {
                    unlocked = true;
                }
                break;    
            
            case GateType. OR:
                if(locksUnlocked > 0)
                {
                    unlocked = true;
                }
                break;    
                    
            case GateType. XOR:
                if (locksUnlocked > 0 && locksUnlocked < lockList.Count())
                {
                    unlocked = true;
                }
                break;    
                    
            case GateType. XNOR:
                if (locksUnlocked == 0 || locksUnlocked == lockList.Count())
                {
                    unlocked = true;
                }
                break;
            default:
                unlocked = false;
                throw new Exception("Default case in " + this.Name);
                    
        }

        opened = unlocked;
        
        if (!unlocked)
        {
            return false;
        }

        door.AttemptToOpen();
        return true;
    }

    public bool Close()
    {
        throw new NotImplementedException();
    }

    public override void UnlockMe(Node2D node)
    {



        if (unlocked == true)
        {
            return;
        }
        unlocked = true;
        door.AttemptToOpen();
    }

    public override void LockMe()
    {

        if (unlocked == false)
        {
            return;
        }
        unlocked = false;
        door.AttemptToOpen();
    }

   
}
