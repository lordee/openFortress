using Godot;
using System;
using System.Collections.Generic;

public class PlayerController
{
    public int NetworkID;
    // Player commands, stores wish commands that the player asks for (Forward, back, jump, etc)
    public Cmd PlayerCommands;
    private Player _player;

    Network _network;
    public PlayerController()
    {

    }

    public void InitPlayer(int networkID)
    {
        NetworkID = networkID;
        _network = (Network)GetNode("/root/OpenFortress/Network");
        // add player node


    }
}

public struct Cmd 
{
    public float move_forward;
    public float move_right;
    public float move_up;
}