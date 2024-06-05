# if TOOLS
using Godot;
using System;

[Tool]
public partial class DamageBoxes2D : EditorPlugin
{
    public override void _EnterTree()
    {
        // Initialization of the plugin goes here.
        // Add the new type with a name, a parent type, a script and an icon.
        var script = GD.Load<Script>("res://addons/hithurtboxes2d/HitBox2D.cs");
        var script2 = GD.Load<Script>("res://addons/hithurtboxes2d/HurtBox2D.cs");
        var texture = GD.Load<Texture2D>("res://addons/hithurtboxes2d/HitHurtBox2D.png");

        AddCustomType("HitBox2D", "Area2D", script, texture);
        AddCustomType("HurtBox2D", "Area2D", script2, texture);
    }

    public override void _ExitTree()
    {
        // Clean-up of the plugin goes here.
        // Always remember to remove it from the engine when deactivated.
        RemoveCustomType("HitBox2D");
        RemoveCustomType("HurtBox2D");
    }
}
#endif