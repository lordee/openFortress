using Godot;
using System;

public class OpenFortress : Node
{
    public override void _Ready()
    {
        PackedScene lobby = (PackedScene)ResourceLoader.Load("res://Scenes/Lobby.tscn");
        Lobby inst = (Lobby)lobby.Instance();
        this.AddChild(inst);
    }
}
