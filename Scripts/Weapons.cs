using Godot;
using System;

// these will no doubt move to their own classes and godot scenes/nodes
// just use these as definitions for now

public enum Ammunition 
{
    Shells,
    Nails,
    Rockets,
    Cells,
    FragGrenade,
    MFTGrenade,
    ConcussionGrenade,
    MIRVGrenade,
    NapalmGrenade,
    NailGrenade,
    GasGrenade,
    EMPGrenade,
    Flare,
    Axe
}

abstract public class Weapon : MeshInstance
{
    protected int _damage;
    protected MeshInstance _mi;
    protected string _scene;
    protected int _minAmmoRequired;
    protected Ammunition _ammoType;
    protected string _resource;

    public Weapon() {

    }

    public string Scene
    {
        get {
            return _scene;
        }
    }

    public string Resource
    {
        get {
            return _resource;
        }
    }

    public MeshInstance Mesh
    {
        get {
            return _mi;
        }
        set {
            _mi = value;
        }
    }
    public Vector3 SpawnTranslation
    {
        get {
            return new Vector3(.5f, -.5f, -.9f);
        }
    }
    public int Damage
    {
        get {
            return _damage;
        }
    }

    public int MinAmmoRequired
    {
        get {
            return _minAmmoRequired;
        }
    }
    public Ammunition AmmoType
    {
        get {
            return _ammoType;
        }
    }

    public void Spawn(Node camera)
    {
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load(Resource);
        Mesh = (MeshInstance)PackedScene.Instance();
        camera.AddChild(Mesh);
        Mesh.Translation = this.SpawnTranslation;
    }
}

public class FragGrenade : Weapon
{
    public FragGrenade() {
        GD.Print("FragGrenade");
        _damage = 100;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.FragGrenade;
        _resource = "res://Shotgun.tscn";
    }
}

// magic fly tool grenade - old concussion
public class MFTGrenade : Weapon
{
    public MFTGrenade() {
        GD.Print("MFTGrenade");
        _damage = 0;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.MFTGrenade;
        _resource = "res://Shotgun.tscn";
    }
}

public class ConcussionGrenade : Weapon
{
    public ConcussionGrenade() {
        GD.Print("ConcussionGrenade");
        _damage = 0;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.ConcussionGrenade;
        _resource = "res://Shotgun.tscn";
    }
}

// this is useless, replace it with something
public class Flare : Weapon
{
    public Flare() {
        GD.Print("Flare");
        _damage = 0;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Flare;
        _resource = "res://Shotgun.tscn";
    }
}

public class NailGrenade : Weapon
{
    public NailGrenade() {
        GD.Print("NailGrenade");
        _damage = 50;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.NailGrenade;
        _resource = "res://Shotgun.tscn";
    }
}

public class MIRVGrenade : Weapon
{
    public MIRVGrenade() {
        GD.Print("MIRVGrenade");
        _damage = 50;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.MIRVGrenade;
        _resource = "res://Shotgun.tscn";
    }
}

public class NapalmGrenade : Weapon
{
    public NapalmGrenade() {
        GD.Print("NapalmGrenade");
        _damage = 20;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.NapalmGrenade;
        _resource = "res://Shotgun.tscn";
    }
}

public class GasGrenade : Weapon
{
    public GasGrenade() {
        GD.Print("GasGrenade");
        _damage = 10;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.GasGrenade;
        _resource = "res://Shotgun.tscn";
    }
}

public class EMPGrenade : Weapon
{
    public EMPGrenade() {
        GD.Print("EMPGrenade");
        _damage = 0;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.EMPGrenade;
        _resource = "res://Shotgun.tscn";
    }
}

public class Axe : Weapon
{
    public Axe() {
        GD.Print("Axe");
        _damage = 25;
        _minAmmoRequired = 0;
        _ammoType = Ammunition.Axe;
        _resource = "res://Shotgun.tscn";
    }
}

public class Shotgun : Weapon
{
    public Shotgun() {
        GD.Print("Shotgun");
        _damage = 25;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Shells;
        _resource = "res://Shotgun.tscn";
    }
}

public class SuperShotgun : Weapon
{
    public SuperShotgun() {
        GD.Print("SuperShotgun");
        _damage = 50;
        _minAmmoRequired = 2;
        _ammoType = Ammunition.Shells;
        _resource = "res://Shotgun.tscn";
    }
}

public class NailGun : Weapon
{
    public NailGun() {
        GD.Print("NailGun");
        _damage = 15;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Nails;
        _resource = "res://Shotgun.tscn";
    }
}

public class SniperRifle : Weapon
{
    public SniperRifle() {
        GD.Print("SniperRifle");
        _damage = 10;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Shells;
        _resource = "res://Shotgun.tscn";
    }
}

public class AutoRifle : Weapon
{
    public AutoRifle() {
        GD.Print("AutoRifle");
        _damage = 10;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Shells;
        _resource = "res://Shotgun.tscn";
    }
}

public class SuperNailGun : Weapon
{
    public SuperNailGun() {
        GD.Print("SuperNailGun");
        _damage = 30;
        _minAmmoRequired = 2;
        _ammoType = Ammunition.Nails;
        _resource = "res://Shotgun.tscn";
    }
}

public class GrenadeLauncher : Weapon
{
    public GrenadeLauncher() {
        GD.Print("GrenadeLauncher");
        _damage = 100;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Rockets;
        _resource = "res://Shotgun.tscn";
    }
}

public class PipebombLauncher : Weapon
{
    public PipebombLauncher() {
        GD.Print("PipebombLauncher");
        _damage = 100;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Rockets;
        _resource = "res://Shotgun.tscn";
    }
}

public class RocketLauncher : Weapon
{
    public RocketLauncher() {
        GD.Print("RocketLauncher");
        _damage = 100;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Rockets;
        _resource = "res://Shotgun.tscn";
    }
}

public class Syringe : Weapon
{
    public Syringe() {
        GD.Print("Syringe");
        _damage = 10;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Axe;
        _resource = "res://Shotgun.tscn";
    }
}

public class MiniGun : Weapon
{
    public MiniGun() {
        GD.Print("MiniGun");
        _damage = 10;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Shells;
        _resource = "res://Shotgun.tscn";
    }
}

public class FlameThrower : Weapon
{
    public FlameThrower() {
        GD.Print("FlameThrower");
        _damage = 10;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Cells;
        _resource = "res://Shotgun.tscn";
    }
}

public class PyroLauncher : Weapon
{
    public PyroLauncher() {
        GD.Print("PyroLauncher");
        _damage = 10;
        _minAmmoRequired = 3;
        _ammoType = Ammunition.Rockets;
        _resource = "res://Shotgun.tscn";
    }
}

public class Tranquiliser : Weapon
{
    public Tranquiliser() {
        GD.Print("Tranquiliser");
        _damage = 10;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Shells;
        _resource = "res://Shotgun.tscn";
    }
}

public class Knife : Weapon
{
    public Knife() {
        GD.Print("Knife");
        _damage = 100;
        _minAmmoRequired = 0;
        _ammoType = Ammunition.Axe;
        _resource = "res://Shotgun.tscn";
    }
}

public class RailGun : Weapon
{
    public RailGun() {
        GD.Print("RailGun");
        _damage = 20;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Nails;
        _resource = "res://Shotgun.tscn";
    }
}

public class Spanner : Weapon
{
    public Spanner() {
        GD.Print("Spanner");
        _damage = 25;
        _minAmmoRequired = 0;
        _ammoType = Ammunition.Axe;
        _resource = "res://Shotgun.tscn";
    }
}

