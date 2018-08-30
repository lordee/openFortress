using Godot;
using System;

public class ClassMenu : VBoxContainer
{

    public override void _Ready()
    {
        
    }
    public void Init(string nodeName)
    {
        var buttons = GetTree().GetNodesInGroup("classbutton");
        foreach (Button b in buttons)
        {
            b.Connect("pressed", this, "_on_Button_Pressed", new object[] { b, nodeName });
        }
    }

    private void _on_Button_Pressed(Button b, string nodeName)
    {
        Player p = (Player)GetNode("/root/OpenFortress/Main/" + nodeName);

        switch (b.Name)
        {
            case "Scout":
                p.Class = new Scout();
            break;
            case "Sniper":
                p.Class = new Sniper();
            break;
            case "Soldier":
                p.Class = new Soldier();
            break;
            case "Demoman":
                p.Class = new Demoman();
            break;
            case "Medic":
                p.Class = new Medic();
            break;
            case "HWGuy":
                p.Class = new HWGuy();
            break;
            case "Pyro":
                p.Class = new Pyro();
            break;
            case "Spy":
                p.Class = new Spy();
            break;
            case "Engineer":
                p.Class = new Engineer();
            break;
            default:
                p.Class = new Observer();
            break;
        }
        GD.Print(b.Name);
        GetTree().QueueDelete(this);
        Input.SetMouseMode(Input.MouseMode.Captured);
    }
}
