using Godot;
using System;

public class Pipebomb : Grenade
{
    public override void _Ready()
    {
        _particleResource = "res://Scenes/Weapons/RocketExplosion.tscn";
        _areaOfEffect = true;
        _areaOfEffectRadius = 5;
        _lifeTime = 60f;
    }
}