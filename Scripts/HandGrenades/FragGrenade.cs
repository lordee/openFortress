using Godot;
using System;
using System.Collections.Generic;



public class FragGrenade : HandGrenade
{
    public static string ProjectileResource = "res://Scenes/HandGrenades/FragGrenade.tscn";

    public FragGrenade()
    {
        _damage = 100;
    }
}