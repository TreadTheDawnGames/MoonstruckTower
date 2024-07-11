using Godot;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

public interface IDoor 
{
     List<ILock> lockList {get; set;}
     bool opened { get; set; }


    public bool AttemptToOpen();
   

    public bool Close();
    

}
