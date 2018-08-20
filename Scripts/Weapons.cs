using Godot;
using System;
using System.Collections.Generic;

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
    protected string _weaponResource;
    protected MeshInstance _weaponMesh;
    protected string _scene;
    protected int _minAmmoRequired;
    protected Ammunition _ammoType;
    protected int _clipSize;
    protected int _clipLeft;
    protected float _timeSinceLastShot;
    protected float _coolDown;
    protected bool _projectile = false;
    protected float _timeSinceReloaded;
    protected float _reloadTime;
    protected string _projectileResource;
    protected Projectile _projectileMesh;
    protected PackedScene _projectileScene;   
    protected int _projectileSpeed;
    
    private Sprite3D muzzleFlash;
    private AudioStreamPlayer3D shootSound;
    private AudioStreamPlayer3D reloadSound;
    private float ShootRange = 1000f;
    Node MainNode;

    public Weapon() {
    }

    // make our own physics process because we do everything via script
    public void PhysicsProcess(float delta)
    {
        this.TimeSinceLastShot += delta;
        this.TimeSinceReloaded += delta;
    }

    public Vector3 SpawnTranslation
    {
        get {
            return new Vector3(.5f, -.5f, -.9f);
        }
    }

    public int ClipLeft
    {
        get {
            return _clipLeft;
        }
        set {
            _clipLeft = _clipSize == -1 ? 999 : value;
        }
    }

    public int AmmoLeft;

    public float TimeSinceLastShot
    {
        get {
            return _timeSinceLastShot;
        }
        set {
            if (value > 0.1f && muzzleFlash != null)
            {
                muzzleFlash.Hide();
            }
            _timeSinceLastShot = value;
        }
    }

    public float TimeSinceReloaded
    {
        get {
            return _timeSinceReloaded;
        }
        set {
            _timeSinceReloaded = value;
            if (_timeSinceReloaded > _reloadTime && this.Reloading)
            {
                this.Reload(true);
            }
        }
    }

    public bool Reloading = false;

    public bool Shoot(Camera camera, Vector2 cameraCenter, Player p) 
    {
        bool shot = false;
        
        // if enough ammunition in clip
        if (ClipLeft >= _minAmmoRequired)
        {
            // if weapon has hit cooldown
            if (TimeSinceLastShot >= _coolDown)
            {
                this.TimeSinceLastShot = 0f;
                ClipLeft -= _minAmmoRequired;
                GD.Print("ClipSize: " + _clipSize);
                GD.Print("ClipLeft: " + ClipLeft);
                // fire either hitscan or projectile
                if (muzzleFlash != null)
                {
                    muzzleFlash.Show();
                }
                shootSound.Play();
                if (_projectile)
                {
                    // spawn projectile, set it moving
                    _projectileMesh = (Projectile)_projectileScene.Instance();
                    
                    // add to scene
                    _weaponMesh.GetNode("/root/Main").AddChild(_projectileMesh);
                    
                    Transform t = camera.GetGlobalTransform();
                    _projectileMesh.Init(t, p, _projectileSpeed, _damage);
                }
                else 
                {
                    PhysicsDirectSpaceState spaceState = p.GetWorld().DirectSpaceState;
                    // null should be self?
                    Vector3 shootOrigin = camera.ProjectRayOrigin(new Vector2(cameraCenter.x, cameraCenter.y));
                    Vector3 shootNormal = camera.ProjectRayNormal(new Vector2(cameraCenter.x, cameraCenter.y)) * ShootRange;
                    Dictionary<object, object> result = spaceState.IntersectRay(shootOrigin, shootNormal, new object[] { this }, 1);

                    Vector3 impulse;
                    Vector3 impact_position;
                    if (result.Count > 0)
                    {
                        impact_position = (Vector3)result["position"];
                        impulse = (impact_position - (Vector3)p.GlobalTransform.origin).Normalized();
                        
                        if (result["collider"] is RigidBody c)
                        {
                            Vector3 position = impact_position - c.GlobalTransform.origin;
                            c.ApplyImpulse(position, impulse * 10);
                        }
                    }
                }
                shot = true;
            }
            else
            {
                shot = false;
            }
        }
        else
        {
            // force a reload
            this.Reload(false);
            shot = false;
        }
        return shot;
    }

    public void Reload(bool reloadFinished)
    {
        if (reloadFinished)
        {
            GD.Print("Reloaded");
            _weaponMesh.SetVisible(true);
            _clipLeft = this.AmmoLeft < _clipSize ? this.AmmoLeft : _clipSize;
            this.Reloading = false;
        } else 
        {
            GD.Print("Reloading...");
            reloadSound.Play();
            _weaponMesh.SetVisible(false);
            this.TimeSinceReloaded = 0f;
            this.Reloading = true;
        }
    }

    public void Spawn(Node camera, string Name)
    {
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load(_weaponResource);
        _weaponMesh = (MeshInstance)PackedScene.Instance();
        camera.AddChild(_weaponMesh);
        _weaponMesh.Translation = this.SpawnTranslation;
        _weaponMesh.SetName(Name);
        _weaponMesh.SetVisible(false);
        shootSound = (AudioStreamPlayer3D)_weaponMesh.GetNode("ShootSound");
        if (_weaponMesh.HasNode("MuzzleFlash"))
        {
            muzzleFlash = (Sprite3D)_weaponMesh.GetNode("MuzzleFlash");
        }
        if (_weaponMesh.HasNode("ReloadSound"))
        {
            reloadSound = (AudioStreamPlayer3D)_weaponMesh.GetNode("ReloadSound");
        }
        
        // projectile mesh
        if (_projectile)
        {
            _projectileScene = (PackedScene)ResourceLoader.Load(_projectileResource);
        }
        
        GD.Print("Loaded " + Name);
    }
}

public class FragGrenade : Weapon
{
    public FragGrenade() {
        GD.Print("FragGrenade");
        _damage = 100;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.FragGrenade;
        _weaponResource = "res://Scenes/Weapons/Shotgun.tscn";
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
        _weaponResource = "res://Scenes/Weapons/Shotgun.tscn";
    }
}

public class ConcussionGrenade : Weapon
{
    public ConcussionGrenade() {
        GD.Print("ConcussionGrenade");
        _damage = 0;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.ConcussionGrenade;
        _weaponResource = "res://Scenes/Weapons/Shotgun.tscn";
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
        _weaponResource = "res://Scenes/Weapons/Shotgun.tscn";
    }
}

public class NailGrenade : Weapon
{
    public NailGrenade() {
        GD.Print("NailGrenade");
        _damage = 50;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.NailGrenade;
        _weaponResource = "res://Scenes/Weapons/Shotgun.tscn";
    }
}

public class MIRVGrenade : Weapon
{
    public MIRVGrenade() {
        GD.Print("MIRVGrenade");
        _damage = 50;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.MIRVGrenade;
        _weaponResource = "res://Scenes/Weapons/Shotgun.tscn";
    }
}

public class NapalmGrenade : Weapon
{
    public NapalmGrenade() {
        GD.Print("NapalmGrenade");
        _damage = 20;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.NapalmGrenade;
        _weaponResource = "res://Scenes/Weapons/Shotgun.tscn";
    }
}

public class GasGrenade : Weapon
{
    public GasGrenade() {
        GD.Print("GasGrenade");
        _damage = 10;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.GasGrenade;
        _weaponResource = "res://Scenes/Weapons/Shotgun.tscn";
    }
}

public class EMPGrenade : Weapon
{
    public EMPGrenade() {
        GD.Print("EMPGrenade");
        _damage = 0;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.EMPGrenade;
        _weaponResource = "res://Scenes/Weapons/Shotgun.tscn";
    }
}

public class Axe : Weapon
{
    public Axe() {
        GD.Print("Axe");
        _damage = 25;
        _minAmmoRequired = 0;
        _ammoType = Ammunition.Axe;
        _weaponResource = "res://Scenes/Weapons/Axe.tscn";
        _coolDown = 0.5f;
    }
}

public class Shotgun : Weapon
{
    public Shotgun() {
        GD.Print("Shotgun");
        _damage = 25;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Shells;
        _weaponResource = "res://Scenes/Weapons/Shotgun.tscn";
        _clipSize = 8;
        _clipLeft = _clipSize == -1 ? 999 : _clipSize;
        _coolDown = 1.0f;
        _reloadTime = 4.0f;
    }
}

public class SuperShotgun : Weapon
{
    public SuperShotgun() {
        GD.Print("SuperShotgun");
        _damage = 50;
        _minAmmoRequired = 2;
        _ammoType = Ammunition.Shells;
        _weaponResource = "res://Scenes/Weapons/SuperShotgun.tscn";
        _clipSize = 16;
        _clipLeft = _clipSize == -1 ? 999 : _clipSize;
        _coolDown = 1.0f;
        _reloadTime = 4.0f;
    }
}

public class NailGun : Weapon
{
    public NailGun() {
        GD.Print("NailGun");
        _damage = 15;
        _minAmmoRequired = 1;
        _weaponResource = "res://Scenes/Weapons/NailGun.tscn";

        _ammoType = Ammunition.Nails;
        _projectile = true;
        _projectileResource = "res://Scenes/Weapons/Nail.tscn";
        _projectileSpeed = 25;
        _clipSize = -1;
        _clipLeft = _clipSize == -1 ? 999 : _clipSize;
        _coolDown = 0.2f;
    }
}

public class SniperRifle : Weapon
{
    public SniperRifle() {
        GD.Print("SniperRifle");
        _damage = 10;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Shells;
        _weaponResource = "res://Scenes/Weapons/SniperRifle.tscn";
    }
}

public class SuperNailGun : NailGun
{
    public SuperNailGun() {
        GD.Print("SuperNailGun");
        _damage = 30;
        _minAmmoRequired = 2;
        _weaponResource = "res://Scenes/Weapons/SuperNailGun.tscn";
    }
}

public class GrenadeLauncher : Weapon
{
    public GrenadeLauncher() {
        GD.Print("GrenadeLauncher");
        _damage = 100;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Rockets;
        _weaponResource = "res://Scenes/Weapons/GrenadeLauncher.tscn";
    }
}

public class PipebombLauncher : Weapon
{
    public PipebombLauncher() {
        GD.Print("PipebombLauncher");
        _damage = 100;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Rockets;
        _weaponResource = "res://Scenes/Weapons/PipebombLauncher.tscn";
    }
}

public class RocketLauncher : Weapon
{
    public RocketLauncher() {
        GD.Print("RocketLauncher");
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Rockets;
        _weaponResource = "res://Scenes/Weapons/RocketLauncher.tscn";
        _projectile = true;
        _projectileResource = "res://Scenes/Weapons/Rocket.tscn";
        _projectileSpeed = 25;
        _damage = 100;
        _clipSize = 4;
        _clipLeft = _clipSize == -1 ? 999 : _clipSize;
        _coolDown = 1.0f;
        _reloadTime = 4.0f;
    }
}

public class Syringe : Weapon
{
    public Syringe() {
        GD.Print("Syringe");
        _damage = 10;
        _minAmmoRequired = 0;
        _ammoType = Ammunition.Axe;
        _weaponResource = "res://Scenes/Weapons/Syringe.tscn";
    }
}

public class MiniGun : Weapon
{
    public MiniGun() {
        GD.Print("MiniGun");
        _damage = 10;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Shells;
        _weaponResource = "res://Scenes/Weapons/MiniGun.tscn";
    }
}

public class FlameThrower : Weapon
{
    public FlameThrower() {
        GD.Print("FlameThrower");
        _damage = 10;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Cells;
        _weaponResource = "res://Scenes/Weapons/FlameThrower.tscn";
    }
}

public class PyroLauncher : Weapon
{
    public PyroLauncher() {
        GD.Print("PyroLauncher");
        _damage = 10;
        _minAmmoRequired = 3;
        _ammoType = Ammunition.Rockets;
        _weaponResource = "res://Scenes/Weapons/PyroLauncher.tscn";
    }
}

public class Tranquiliser : Weapon
{
    public Tranquiliser() {
        GD.Print("Tranquiliser");
        _damage = 10;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Shells;
        _weaponResource = "res://Scenes/Weapons/Tranquiliser.tscn";
    }
}

public class Knife : Weapon
{
    public Knife() {
        GD.Print("Knife");
        _damage = 100;
        _minAmmoRequired = 0;
        _ammoType = Ammunition.Axe;
        _weaponResource = "res://Scenes/Weapons/Knife.tscn";
    }
}

public class RailGun : Weapon
{
    public RailGun() {
        GD.Print("RailGun");
        _damage = 20;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Nails;
        _weaponResource = "res://Scenes/Weapons/RailGun.tscn";
    }
}

public class Spanner : Weapon
{
    public Spanner() {
        GD.Print("Spanner");
        _damage = 25;
        _minAmmoRequired = 0;
        _ammoType = Ammunition.Axe;
        _weaponResource = "res://Scenes/Weapons/Spanner.tscn";
    }
}

