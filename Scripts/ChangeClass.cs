using Godot;
using System;

public class ChangeClass : VBoxContainer
{
    // first set when instanced by server.cs
    public Client Client = null;

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
        
    }
    
    private void SetClass(string c)
    {
        Client.playerClass = c;
        Server s = (Server)GetNode("/root/OpenFortress/Main/Server");
        s.SpawnPlayer(Client);
        Client.Player.SetInMenu(false);
        GetTree().QueueDelete(this);
    }

    public void _on_Scout_pressed()
    {
        this.SetClass("scout");
    }
    public void _on_Sniper_pressed()
    {
        this.SetClass("sniper");
    }
    public void _on_Soldier_pressed()
    {
        this.SetClass("soldier");
    }
    public void _on_Demoman_pressed()
    {
        this.SetClass("demoman");
    }
    public void _on_Medic_pressed()
    {
        this.SetClass("medic");
    }
    public void _on_HWguy_pressed()
    {
        this.SetClass("hwguy");
    }
    public void _on_Pyro_pressed()
    {
        this.SetClass("pyro");
    }
    public void _on_Spy_pressed()
    {
        this.SetClass("spy");
    }
    public void _on_Engineer_pressed()
    {
        this.SetClass("engineer");
    }
    public void _on_Random_pressed()
    {
        this.SetClass("random");
    }

    public void _on_Cancel_pressed()
    {
        GetTree().QueueDelete(this);
    }
}
