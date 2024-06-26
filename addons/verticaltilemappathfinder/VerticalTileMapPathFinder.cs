# if TOOLS
using Godot;
using System;

[Tool]
public partial class VerticalTileMapPathFinder : EditorPlugin
{
    public override void _EnterTree()
    {
        // Initialization of the plugin goes here.
        // Add the new type with a name, a parent type, a script and an icon.
        var script = GD.Load<Script>("res://addons/VerticalTileMapPathFinder/TileMapPathFind.cs");
        var playerScript = GD.Load<Script>("res://addons/VerticalTileMapPathFinder/PlayerPathfinder.cs");

        var pathFinderImage = GD.Load<Texture2D>("res://addons/VerticalTileMapPathFinder/PathFinderImage.png");
        var playerPathFinderImage = GD.Load<Texture2D>("res://addons/VerticalTileMapPathFinder/PlayerPathFinderImage.png");

        AddCustomType("TileMapPathFind", "TileMap", script, pathFinderImage);
        AddCustomType("PlayerPathfinder", "CharacterBody2D", playerScript, playerPathFinderImage);
    }

    public override void _ExitTree()
    {
        // Clean-up of the plugin goes here.
        // Always remember to remove it from the engine when deactivated.
        RemoveCustomType("TileMapPathFind");
        RemoveCustomType("PlayerPathfinder");
    }
}
#endif
