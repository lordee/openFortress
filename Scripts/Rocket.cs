using Godot;
using System;

public class Rocket : Projectile
{
    public override void _Ready()
    {
        _particleResource = "res://Scenes/Weapons/RocketExplosion.tscn";
        _areaOfEffect = true;
        _areaOfEffectRadius = 5;
        _projectileType = "Rocket";
    }
}
