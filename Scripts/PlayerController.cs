using Godot;
using System;
using System.Collections.Generic;

public struct PlayerController
{
    // Player commands, stores wish commands that the player asks for (Forward, back, jump, etc)
    public float move_forward;
    public float move_right;
    public float move_up;
    public List<Impulse> Impulses;
}

public enum Impulse
{
    Slot1
    , Slot2
    , Slot3
    , Slot4
    , Detpipe
    , Gren1
    , Gren2
    , Attack
}