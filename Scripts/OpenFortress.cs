using Godot;
using System;

public class OpenFortress : Node
{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
        PackedScene lobby = (PackedScene)ResourceLoader.Load("res://Scenes/Lobby.tscn");
        Lobby inst = (Lobby)lobby.Instance();
        this.AddChild(inst);
    }
}
