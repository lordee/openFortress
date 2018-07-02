using Godot;
using System;

public class TeamMenu : VBoxContainer
{

    public override void _Ready()
    {
        Button b = new Button();
        b.Text = "Team 1";
        b.SizeFlagsVertical = (int)SizeFlags.ExpandFill;
        this.AddChild(b);
        b.Connect("pressed", this, "JoinTeam", new object[] {1});
        Button b2 = new Button();
        b2.Text = "Team 2";
        b2.SizeFlagsVertical = (int)SizeFlags.ExpandFill;
        this.AddChild(b);
        b2.Connect("pressed", this, "JoinTeam", new object[] {2});
        Button exit = new Button();
        exit.Text = "Cancel";
        this.AddChild(exit);
        exit.Connect("pressed", this, "ExitMenu");
    }

    public void JoinTeam(int teamID)
    {
        
        
        GetTree().QueueDelete(this);
    }

    public void ExitMenu()
    {
       
    }
}
