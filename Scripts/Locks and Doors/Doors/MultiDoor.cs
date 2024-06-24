using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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
    }

    public override bool Open()
    {
        if (!base.Open())
        {
            return false;
        }

        foreach (Door door in doors)
        {
            door.opened = true;
            door.Open();
        }
        return true;
    }

    public override bool Close()
    {
        if (base.Close())
        {
            return false;
        }

        foreach (var door in doors)
        {
            door.opened = false;
            door.Close() ;
        }
        return true;
    }

   
}
