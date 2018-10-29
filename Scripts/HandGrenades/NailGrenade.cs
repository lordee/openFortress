using Godot;
using System;
using System.Collections.Generic;



public class NailGrenade : HandGrenade
{
    _damage = 30;
    public static float Damage = 0;
    public static string ProjectileResource = "res://Scenes/HandGrenades/NailGrenade.tscn";

    public NailGrenade()
    {
    }

    public void Stage1()
    {
        // grenade rises from ground
        // grenade rotates
        // grenade fires nails for lifetime
        // when lifetime ends, call explode
    }
}