using Godot;
using System;

public class Network : Node
{
    

    public override void _Ready()
    {

        
    }

    public override void _Process(float delta)
    {
        if (this.IsNetworkMaster())
        {
            // check if there is new data to send clients
            // send data to each client
        }
        else
        {
            // check if there is new data
            // send to server
        }
    }
}
