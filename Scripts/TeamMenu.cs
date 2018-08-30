using Godot;
using System;

public class TeamMenu : VBoxContainer
{

    public override void _Ready()
    {
        
    }

    public void Init(string playerNode)
    {
        Input.SetMouseMode(Input.MouseMode.Visible);
        Button b = new Button();
        b.Text = "Team 1";
        b.SizeFlagsVertical = (int)SizeFlags.ExpandFill;
        this.AddChild(b);
        b.Connect("pressed", this, "JoinTeam", new object[] {1, playerNode});
        Button b2 = new Button();
        b2.Text = "Team 2";
        b2.SizeFlagsVertical = (int)SizeFlags.ExpandFill;
        this.AddChild(b2);
        b2.Connect("pressed", this, "JoinTeam", new object[] {2, playerNode});
        Button b9 = new Button();
        b9.Text = "Spectator";
        b9.SizeFlagsVertical = (int)SizeFlags.ExpandFill;
        this.AddChild(b9);
        b9.Connect("pressed", this, "JoinTeam", new object[] {9, playerNode});
        Button exit = new Button();
        exit.Text = "Cancel";
        this.AddChild(exit);
        exit.Connect("pressed", this, "ExitMenu");
    }

    public void JoinTeam(int teamID, string nodeName)
    {
        Player p = (Player)GetNode("/root/OpenFortress/Main/" + nodeName);
        p.TeamID = teamID;

        PackedScene c = (PackedScene)ResourceLoader.Load("res://Scenes/ClassMenu.tscn");
        ClassMenu c2 = (ClassMenu)c.Instance();
        Node main = GetNode("/root/OpenFortress/Main/");
        main.AddChild(c2);
        c2.Init(nodeName);

        GetTree().QueueDelete(this);
    }

    public void ExitMenu()
    {
       GetTree().QueueDelete(this);
    }
}
