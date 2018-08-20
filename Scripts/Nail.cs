using Godot;
using System;

public class Nail : Projectile
{
    public override void _Ready()
    {
        _particleResource = "res://Scenes/Weapons/NailTink.tscn";
        _areaOfEffect = false;
        _projectileType = "nail";
    }
}