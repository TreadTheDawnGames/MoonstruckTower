using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

[Icon("res://Assets/Locks and Doors/Icons/MultiDoorIcon.png")]
public partial class MultiDoor : Door
{
    [Export] bool forced = false;
    List<IDoor> doors = new();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        foreach (IDoor door in GetChildren().OfType<IDoor>())
        {
            doors.Add(door);
        }
        base._Ready();
        foreach (IDoor door in doors)
        {
            door.lockList = lockList;
        }
    }

    public override bool AttemptToOpen()
    {
        foreach (IDoor door in doors)
        {
            if (forced)
            {
                door.type = IDoor.GateType.FORCED;
            }

            /*if (!base.AttemptToOpen())
            {*/
               door.AttemptToOpen();
            //}
            /*else
            {
                door.Close();
            }*/
        }
        return true;
    }

    /*public override bool Close()
    {
        

        foreach (var door in doors)
        {
            door.AttemptToOpen();
        }
        return true;
    }*/

   
}
