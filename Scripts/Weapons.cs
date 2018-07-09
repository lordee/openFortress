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
    abstract public MeshInstance mi
    {
        get; set;
    }
    public Vector3 SpawnTranslation
    {
        get {
            return new Vector3(.5f, -.5f, -.9f);
        }
    }
    abstract public int Damage
    {
        get;
    }

    abstract public int MinAmmoRequired
    {
        get;
    }
    abstract public Ammunition AmmoType
    {
        get;
    }

    public void Spawn(Node camera)
    {
        camera.AddChild(mi);
        mi.Translation = this.SpawnTranslation;
    }
}

public class FragGrenade : Weapon
{
    override public MeshInstance mi
    {
        get; set;
    }

    public FragGrenade() {
        GD.Print("FragGrenade");
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load("res://Shotgun.tscn");
        mi = (MeshInstance)PackedScene.Instance();
    }

    override public int Damage
    {
        get {
            return 100;
        }
    }
    override public int MinAmmoRequired
    {
        get {
            return 1;
        }
    }
    override public Ammunition AmmoType
    {
        get {
            return Ammunition.FragGrenade;
        }
    }
}

// magic fly tool grenade - old concussion
public class MFTGrenade : Weapon
{
    override public MeshInstance mi
    {
        get; set;
    }

    public MFTGrenade() {
        GD.Print("MFTGrenade");
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load("res://Shotgun.tscn");
        mi = (MeshInstance)PackedScene.Instance();
    }

    override public int Damage
    {
        get {
            return 0;
        }
    }
    override public int MinAmmoRequired
    {
        get {
            return 1;
        }
    }
    override public Ammunition AmmoType
    {
        get {
            return Ammunition.MFTGrenade;
        }
    }
}

public class ConcussionGrenade : Weapon
{
    override public MeshInstance mi
    {
        get; set;
    }

    public ConcussionGrenade() {
        GD.Print("ConcussionGrenade");
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load("res://Shotgun.tscn");
        mi = (MeshInstance)PackedScene.Instance();
    }

    override public int Damage
    {
        get {
            return 0;
        }
    }
    override public int MinAmmoRequired
    {
        get {
            return 1;
        }
    }
    override public Ammunition AmmoType
    {
        get {
            return Ammunition.ConcussionGrenade;
        }
    }
}

// this is useless, replace it with something
public class Flare : Weapon
{
    override public MeshInstance mi
    {
        get; set;
    }

    public Flare() {
        GD.Print("Flare");
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load("res://Shotgun.tscn");
        mi = (MeshInstance)PackedScene.Instance();
    }

    override public int Damage
    {
        get {
            return 0;
        }
    }
    override public int MinAmmoRequired
    {
        get {
            return 1;
        }
    }
    override public Ammunition AmmoType
    {
        get {
            return Ammunition.Flare;
        }
    }
}

public class NailGrenade : Weapon
{
    override public MeshInstance mi
    {
        get; set;
    }

    public NailGrenade() {
        GD.Print("NailGrenade");
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load("res://Shotgun.tscn");
        mi = (MeshInstance)PackedScene.Instance();
    }

    override public int Damage
    {
        get {
            return 50;
        }
    }
    override public int MinAmmoRequired
    {
        get {
            return 1;
        }
    }
    override public Ammunition AmmoType
    {
        get {
            return Ammunition.NailGrenade;
        }
    }
}

public class MIRVGrenade : Weapon
{
    override public MeshInstance mi
    {
        get; set;
    }

    public MIRVGrenade() {
        GD.Print("MIRVGrenade");
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load("res://Shotgun.tscn");
        mi = (MeshInstance)PackedScene.Instance();
    }

    override public int Damage
    {
        get {
            return 50;
        }
    }
    override public int MinAmmoRequired
    {
        get {
            return 1;
        }
    }
    override public Ammunition AmmoType
    {
        get {
            return Ammunition.MIRVGrenade;
        }
    }
}

public class NapalmGrenade : Weapon
{
    override public MeshInstance mi
    {
        get; set;
    }

    public NapalmGrenade() {
        GD.Print("NapalmGrenade");
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load("res://Shotgun.tscn");
        mi = (MeshInstance)PackedScene.Instance();
    }

    override public int Damage
    {
        get {
            return 20;
        }
    }
    override public int MinAmmoRequired
    {
        get {
            return 1;
        }
    }
    override public Ammunition AmmoType
    {
        get {
            return Ammunition.NapalmGrenade;
        }
    }
}

public class GasGrenade : Weapon
{
    override public MeshInstance mi
    {
        get; set;
    }

    public GasGrenade() {
        GD.Print("GasGrenade");
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load("res://Shotgun.tscn");
        mi = (MeshInstance)PackedScene.Instance();
    }

    override public int Damage
    {
        get {
            return 10;
        }
    }
    override public int MinAmmoRequired
    {
        get {
            return 1;
        }
    }
    override public Ammunition AmmoType
    {
        get {
            return Ammunition.GasGrenade;
        }
    }
}

public class EMPGrenade : Weapon
{
    override public MeshInstance mi
    {
        get; set;
    }

    public EMPGrenade() {
        GD.Print("EMPGrenade");
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load("res://Shotgun.tscn");
        mi = (MeshInstance)PackedScene.Instance();
    }

    override public int Damage
    {
        get {
            return 0;
        }
    }
    override public int MinAmmoRequired
    {
        get {
            return 1;
        }
    }
    override public Ammunition AmmoType
    {
        get {
            return Ammunition.EMPGrenade;
        }
    }
}

public class Axe : Weapon
{
    override public MeshInstance mi
    {
        get; set;
    }

    public Axe() {
        GD.Print("Axe");
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load("res://Shotgun.tscn");
        mi = (MeshInstance)PackedScene.Instance();
    }

    override public int Damage
    {
        get {
            return 10;
        }
    }
    override public int MinAmmoRequired
    {
        get {
            return 0;
        }
    }
    override public Ammunition AmmoType
    {
        get {
            return Ammunition.Axe;
        }
    }
}

public class Shotgun : Weapon
{
    override public MeshInstance mi
    {
        get; set;
    }

    public Shotgun() {
        GD.Print("Shotgun");
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load("res://Shotgun.tscn");
        mi = (MeshInstance)PackedScene.Instance();
    }

    override public int Damage
    {
        get {
            return 25;
        }
    }
    override public int MinAmmoRequired
    {
        get {
            return 1;
        }
    }
    override public Ammunition AmmoType
    {
        get {
            return Ammunition.Shells;
        }
    }
}

public class SuperShotgun : Weapon
{
    override public MeshInstance mi
    {
        get; set;
    }

    public SuperShotgun() {
        GD.Print("SuperShotgun");
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load("res://Shotgun.tscn");
        mi = (MeshInstance)PackedScene.Instance();
    }

    override public int Damage
    {
        get {
            return 50;
        }
    }
    override public int MinAmmoRequired
    {
        get {
            return 2;
        }
    }
    override public Ammunition AmmoType
    {
        get {
            return Ammunition.Shells;
        }
    }
}

public class NailGun : Weapon
{
    override public MeshInstance mi
    {
        get; set;
    }

    public NailGun() {
        GD.Print("NailGun");
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load("res://Shotgun.tscn");
        mi = (MeshInstance)PackedScene.Instance();
    }

    override public int Damage
    {
        get {
            return 15;
        }
    }
    override public int MinAmmoRequired
    {
        get {
            return 1;
        }
    }
    override public Ammunition AmmoType
    {
        get {
            return Ammunition.Nails;
        }
    }
}

public class SniperRifle : Weapon
{
    override public MeshInstance mi
    {
        get; set;
    }

    public SniperRifle() {
        GD.Print("SniperRifle");
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load("res://Shotgun.tscn");
        mi = (MeshInstance)PackedScene.Instance();
    }

    override public int Damage
    {
        get {
            return 10;
        }
    }
    override public int MinAmmoRequired
    {
        get {
            return 1;
        }
    }
    override public Ammunition AmmoType
    {
        get {
            return Ammunition.Shells;
        }
    }
}

public class AutoRifle : Weapon
{
    override public MeshInstance mi
    {
        get; set;
    }

    public AutoRifle() {
        GD.Print("AutoRifle");
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load("res://Shotgun.tscn");
        mi = (MeshInstance)PackedScene.Instance();
    }

    override public int Damage
    {
        get {
            return 10;
        }
    }
    override public int MinAmmoRequired
    {
        get {
            return 1;
        }
    }
    override public Ammunition AmmoType
    {
        get {
            return Ammunition.Shells;
        }
    }
}

public class SuperNailGun : Weapon
{
    override public MeshInstance mi
    {
        get; set;
    }

    public SuperNailGun() {
        GD.Print("SuperNailGun");
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load("res://Shotgun.tscn");
        mi = (MeshInstance)PackedScene.Instance();
    }

    override public int Damage
    {
        get {
            return 30;
        }
    }
    override public int MinAmmoRequired
    {
        get {
            return 2;
        }
    }
    override public Ammunition AmmoType
    {
        get {
            return Ammunition.Nails;
        }
    }
}

public class GrenadeLauncher : Weapon
{
    override public MeshInstance mi
    {
        get; set;
    }

    public GrenadeLauncher() {
        GD.Print("GrenadeLauncher");
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load("res://Shotgun.tscn");
        mi = (MeshInstance)PackedScene.Instance();
    }

    override public int Damage
    {
        get {
            return 100;
        }
    }
    override public int MinAmmoRequired
    {
        get {
            return 1;
        }
    }
    override public Ammunition AmmoType
    {
        get {
            return Ammunition.Rockets;
        }
    }
}

public class PipebombLauncher : Weapon
{
    override public MeshInstance mi
    {
        get; set;
    }

    public PipebombLauncher() {
        GD.Print("PipebombLauncher");
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load("res://Shotgun.tscn");
        mi = (MeshInstance)PackedScene.Instance();
    }

    override public int Damage
    {
        get {
            return 100;
        }
    }
    override public int MinAmmoRequired
    {
        get {
            return 1;
        }
    }
    override public Ammunition AmmoType
    {
        get {
            return Ammunition.Rockets;
        }
    }
}

public class RocketLauncher : Weapon
{
    override public MeshInstance mi
    {
        get; set;
    }

    public RocketLauncher() {
        GD.Print("RocketLauncher");
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load("res://Shotgun.tscn");
        mi = (MeshInstance)PackedScene.Instance();
    }

    override public int Damage
    {
        get {
            return 100;
        }
    }
    override public int MinAmmoRequired
    {
        get {
            return 1;
        }
    }
    override public Ammunition AmmoType
    {
        get {
            return Ammunition.Rockets;
        }
    }
}

public class Syringe : Weapon
{
    override public MeshInstance mi
    {
        get; set;
    }

    public Syringe() {
        GD.Print("Syringe");
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load("res://Shotgun.tscn");
        mi = (MeshInstance)PackedScene.Instance();
    }

    override public int Damage
    {
        get {
            return 10;
        }
    }
    override public int MinAmmoRequired
    {
        get {
            return 0;
        }
    }
    override public Ammunition AmmoType
    {
        get {
            return Ammunition.Axe;
        }
    }
}

public class MiniGun : Weapon
{
    override public MeshInstance mi
    {
        get; set;
    }

    public MiniGun() {
        GD.Print("MiniGun");
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load("res://Shotgun.tscn");
        mi = (MeshInstance)PackedScene.Instance();
    }

    override public int Damage
    {
        get {
            return 10;
        }
    }
    override public int MinAmmoRequired
    {
        get {
            return 1;
        }
    }
    override public Ammunition AmmoType
    {
        get {
            return Ammunition.Shells;
        }
    }
}

public class FlameThrower : Weapon
{
    override public MeshInstance mi
    {
        get; set;
    }

    public FlameThrower() {
        GD.Print("FlameThrower");
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load("res://Shotgun.tscn");
        mi = (MeshInstance)PackedScene.Instance();
    }

    override public int Damage
    {
        get {
            return 10;
        }
    }
    override public int MinAmmoRequired
    {
        get {
            return 1;
        }
    }
    override public Ammunition AmmoType
    {
        get {
            return Ammunition.Cells;
        }
    }
}

public class PyroLauncher : Weapon
{
    override public MeshInstance mi
    {
        get; set;
    }

    public PyroLauncher() {
        GD.Print("PyroLauncher");
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load("res://Shotgun.tscn");
        mi = (MeshInstance)PackedScene.Instance();
    }

    override public int Damage
    {
        get {
            return 10;
        }
    }
    override public int MinAmmoRequired
    {
        get {
            return 3;
        }
    }
    override public Ammunition AmmoType
    {
        get {
            return Ammunition.Rockets;
        }
    }
}

public class Tranquiliser : Weapon
{
    override public MeshInstance mi
    {
        get; set;
    }

    public Tranquiliser() {
        GD.Print("Tranquiliser");
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load("res://Shotgun.tscn");
        mi = (MeshInstance)PackedScene.Instance();
    }

    override public int Damage
    {
        get {
            return 10;
        }
    }
    override public int MinAmmoRequired
    {
        get {
            return 1;
        }
    }
    override public Ammunition AmmoType
    {
        get {
            return Ammunition.Shells;
        }
    }
}

public class Knife : Weapon
{
    override public MeshInstance mi
    {
        get; set;
    }

    public Knife() {
        GD.Print("Knife");
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load("res://Shotgun.tscn");
        mi = (MeshInstance)PackedScene.Instance();
    }

    override public int Damage
    {
        get {
            return 100;
        }
    }
    override public int MinAmmoRequired
    {
        get {
            return 0;
        }
    }
    override public Ammunition AmmoType
    {
        get {
            return Ammunition.Axe;
        }
    }
}

public class RailGun : Weapon
{
    override public MeshInstance mi
    {
        get; set;
    }

    public RailGun() {
        GD.Print("RailGun");
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load("res://Shotgun.tscn");
        mi = (MeshInstance)PackedScene.Instance();
    }

    override public int Damage
    {
        get {
            return 20;
        }
    }
    override public int MinAmmoRequired
    {
        get {
            return 1;
        }
    }
    override public Ammunition AmmoType
    {
        get {
            return Ammunition.Nails;
        }
    }
}

public class Spanner : Weapon
{
    override public MeshInstance mi
    {
        get; set;
    }

    public Spanner() {
        GD.Print("Spanner");
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load("res://Shotgun.tscn");
        mi = (MeshInstance)PackedScene.Instance();
    }

    override public int Damage
    {
        get {
            return 10;
        }
    }
    override public int MinAmmoRequired
    {
        get {
            return 1;
        }
    }
    override public Ammunition AmmoType
    {
        get {
            return Ammunition.Axe;
        }
    }
}

