using Godot;
using System;

public class ClassMenu : VBoxContainer
{

    public override void _Ready()
    {
        var buttons = GetTree().GetNodesInGroup("classbutton");
        foreach (Button b in buttons)
        {
            b.Connect("pressed", this, "_on_Button_Pressed", new object[] { b });
        }
    }

    private void _on_Button_Pressed(Button b)
    {
        Player p = (Player)GetNode("/root/Main/Player");
        // ugly
        p.Class = b.Name;

        GetTree().QueueDelete(this);
        Input.SetMouseMode(Input.MouseMode.Captured);
    }
}
