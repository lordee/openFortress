using Godot;
using System;
using System.Collections.Generic;



abstract public class HandGrenade : Weapon
{
    public bool Primed = false;
    private float _primedTime = 0;
    private Player _shooter;
    private Camera _camera;

    public HandGrenade()
    {
        _projectileSpeed = 20;
    }

    override public void PhysicsProcess(float delta)
    {
        _primedTime += delta;

        if (this.Primed && _primedTime >= 3)
        {
            this.Primed = false;
            // explode on player
            _projectileMesh = (FragGrenadeO)_projectileScene.Instance();
            _shooter.AddChild(_projectileMesh);

            _projectileMesh.Init(_camera.GetGlobalTransform(), _shooter, this, _projectileSpeed, _damage);
            _projectileMesh.Explode(null, _damage);
        }
    }

    public override bool Shoot(Camera camera, Vector2 cameraCenter, Player shooter) 
    {
        bool shot = false;
        _shooter = shooter;
        _camera = camera;
        if (Primed)
        {
            this.Primed = false;
            // throw it, TODO test for tranq in future
            // spawn projectile, set it moving
            _projectileMesh = (Projectile)_projectileScene.Instance();
            
            // add to scene
            shooter.MainNode.AddChild(_projectileMesh);
            
            _projectileMesh.Init(camera.GetGlobalTransform(), shooter, this, _projectileSpeed, _damage);
            FragGrenadeO o = (FragGrenadeO)_projectileMesh;
            o.Time = _primedTime;
            shot = true;
        }
        else
        {
            this.Primed = true;
            _primedTime = 0;
            // play grentimer
            AudioStreamPlayer gt = (AudioStreamPlayer)shooter.GetNode("GrenTimer");
            gt.Play();
            shot = true;
        }
        return shot;
    }

    public void Spawn()
    {
        _projectileScene = (PackedScene)ResourceLoader.Load(_projectileResource);
    }
}

public class FragGrenade : HandGrenade
{
    public FragGrenade() {
        GD.Print("FragGrenade");
        _damage = 100;
        _ammoType = Ammunition.FragGrenade;
        _projectileResource = "res://Scenes/Weapons/FragGrenade.tscn";
        base.Spawn();
    }
}

// magic fly tool grenade - old concussion
public class MFTGrenade : HandGrenade
{
    public MFTGrenade() {
        GD.Print("MFTGrenade");
        _damage = 0;
        _ammoType = Ammunition.MFTGrenade;
        _projectileResource = "res://Scenes/Weapons/Shotgun.tscn";
    }
}

public class ConcussionGrenade : HandGrenade
{
    public ConcussionGrenade() {
        GD.Print("ConcussionGrenade");
        _damage = 0;
        _ammoType = Ammunition.ConcussionGrenade;
        _projectileResource = "res://Scenes/Weapons/Shotgun.tscn";
    }
}

// this is useless, replace it with something
public class Flare : HandGrenade
{
    public Flare() {
        GD.Print("Flare");
        _damage = 0;
        _ammoType = Ammunition.Flare;
        _projectileResource = "res://Scenes/Weapons/Shotgun.tscn";
    }
}

public class NailGrenade : HandGrenade
{
    public NailGrenade() {
        GD.Print("NailGrenade");
        _damage = 0;
        _ammoType = Ammunition.NailGrenade;
        _projectileResource = "res://Scenes/Weapons/Shotgun.tscn";
    }
}

public class MIRVGrenade : HandGrenade
{
    public MIRVGrenade() {
        GD.Print("MIRVGrenade");
        _damage = 30;
        _ammoType = Ammunition.MIRVGrenade;
        _projectileResource = "res://Scenes/Weapons/Shotgun.tscn";
    }
}

public class NapalmGrenade : HandGrenade
{
    public NapalmGrenade() {
        GD.Print("NapalmGrenade");
        _damage = 20;
        _ammoType = Ammunition.NapalmGrenade;
        _projectileResource = "res://Scenes/Weapons/Shotgun.tscn";
    }
}

public class GasGrenade : HandGrenade
{
    public GasGrenade() {
        GD.Print("GasGrenade");
        _damage = 0;
        _ammoType = Ammunition.GasGrenade;
        _projectileResource = "res://Scenes/Weapons/Shotgun.tscn";
    }
}

public class EMPGrenade : HandGrenade
{
    public EMPGrenade() {
        GD.Print("EMPGrenade");
        _damage = 0;
        _ammoType = Ammunition.EMPGrenade;
        _projectileResource = "res://Scenes/Weapons/Shotgun.tscn";
    }
}