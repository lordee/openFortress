using Godot;
using System;
using System.Collections.Generic;



public class ConcussionGrenade : HandGrenade
{
    public static string ProjectileResource = "res://Scenes/HandGrenades/ConcussionGrenade.tscn";
    private float _blastPower = 30;

    public ConcussionGrenade()
    {
        _damage = 0;
        _inflictLength = 10;
    }

    override public void PrimeTimeFinished()
    {
        // when lifetime ends, call explode
        this.Explode(false, _blastPower);
    }
}