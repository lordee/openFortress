using Godot;
using System;
using System.Collections.Generic;



public class NailGrenade : HandGrenade
{
    public static string ProjectileResource = "res://Scenes/HandGrenades/NailGrenade.tscn";

    public NailGrenade()
    {
        _damage = 30;
    }

    override public void PrimeTimeFinished()
    {
        // grenade rises from ground
        // grenade rotates
        // grenade fires nails for lifetime
        // when lifetime ends, call explode
        this.Explode(true, _damage);
    }

}