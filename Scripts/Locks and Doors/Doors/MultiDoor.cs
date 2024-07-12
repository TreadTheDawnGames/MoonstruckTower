using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

[Icon("res://Assets/Locks and Doors/Icons/MultiDoorIcon.png")]
public partial class MultiDoor : Door
{

    List<Door> doors = new();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        foreach (Door door in GetChildren().OfType<Door>())
        {
            doors.Add(door);
        }
        base._Ready();
    }

    public override bool AttemptToOpen()
    {
        foreach (Door door in doors)
        {
            if (!base.AttemptToOpen())
            {
               door.AttemptToOpen();
            }
            else
            {
                door.Close();
            }
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
