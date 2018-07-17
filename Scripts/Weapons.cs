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
    
    

    private Sprite3D muzzleFlash;
    private AudioStreamPlayer3D shootSound;
    private AudioStreamPlayer3D reloadSound;
    private float ShootRange = 1000f;
    Node MainNode;

    public Weapon() {
    }

    public string Scene
    {
        get {
            return _scene;
        }
    }

    public string WeaponResource
    {
        get {
            return _weaponResource;
        }
    }

    public MeshInstance WeaponMesh
    {
        get {
            return _weaponMesh;
        }
        set {
            _weaponMesh = value;
        }
    }

    public string ProjectileResource
    {
        get {
            return _projectileResource;
        }
    }

    public PackedScene ProjectileScene
    {
        get {
            return _projectileScene;
        }
        set {
            _projectileScene = value;
        }
    }

    public Projectile ProjectileMesh
    {
        get {
            return _projectileMesh;
        }
        set {
            _projectileMesh = value;
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

    public int ClipLeft
    {
        get {
            return _clipLeft;
        }
        set {
            _clipLeft = ClipSize == -1 ? 999 : value;
        }
    }

    public int ClipSize
    {
        get {
            return _clipSize;
        }
    }

    public float TimeSinceLastShot
    {
        get {
            return _timeSinceLastShot;
        }
        set {
            if (value > 0.1f)
            {
                muzzleFlash.Hide();
            }
            _timeSinceLastShot = value;
        }
    }

    public float CoolDown
    {
        get {
            return _coolDown;
        }
    }

    public bool Projectile
    {
        get {
            return _projectile;
        }
    }

    public float TimeSinceReloaded
    {
        get {
            return _timeSinceReloaded;
        }
        set {
            _timeSinceReloaded = value;
            if (_timeSinceReloaded > ReloadTime)
            {
                this.Reload(true);
            }
        }
    }

    public float ReloadTime
    {
        get {
            return _reloadTime;
        }
    }

    public bool Shoot(Camera camera, Vector2 cameraCenter) 
    {
        bool shot = false;
        // if weapon has hit cooldown
        if (TimeSinceLastShot >= CoolDown)
        {
            // if enough ammunition in clip
            GD.Print("ClipSize: " + ClipSize);
            GD.Print("ClipLeft: " + ClipLeft);
            if (ClipLeft >= MinAmmoRequired)
            {
                ClipLeft -= MinAmmoRequired;
                // fire either hitscan or projectile
                muzzleFlash.Show();
                if (Projectile)
                {
                    // spawn projectile, set it moving
                    GD.Print("ProjectileScene: " + ProjectileScene);
                    ProjectileMesh = (Projectile)ProjectileScene.Instance();
                    
                    //ProjectileMesh.Init();
                    ProjectileMesh.Translation = camera.ProjectRayOrigin(new Vector2(cameraCenter.x, cameraCenter.y));
                    ProjectileMesh.Init(camera.GetGlobalTransform().basis);
                    
                    //ProjectileMesh.Destination = camera.ProjectRayNormal(new Vector2(cameraCenter.x, cameraCenter.y)), new Vector3(0,1,0);
                    //ProjectileMesh.LookAt(camera.ProjectRayNormal(new Vector2(cameraCenter.x, cameraCenter.y)), new Vector3(0,1,0));
                    // set direction
                    //ProjectileMesh.SetRotationDegrees(camera.GetRotationDegrees());


                    // add to scene
                    MainNode.AddChild(ProjectileMesh);
                }
                else 
                {
                    shootSound.Play();
                    PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;
                    // null should be self?
                    Vector3 shootOrigin = camera.ProjectRayOrigin(new Vector2(cameraCenter.x, cameraCenter.y));
                    Vector3 shootNormal = camera.ProjectRayNormal(new Vector2(cameraCenter.x, cameraCenter.y)) * ShootRange;
                    Dictionary<object, object> result = spaceState.IntersectRay(shootOrigin, shootNormal, new object[] { this }, 1);

                    Vector3 impulse;
                    Vector3 impact_position;
                    if (result.Count > 0)
                    {
                        impact_position = (Vector3)result["position"];
                        impulse = (impact_position - (Vector3)GlobalTransform.origin).Normalized();
                        
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
                // force a reload
                this.Reload(false);
                shot = false;
            }
        }
        else
        {
            shot = false;
        }
        return shot;
    }

    public void Reload(bool reloadFinished)
    {
        if (reloadFinished)
        {
            GD.Print("Reloaded");
            WeaponMesh.SetVisible(true);
        } else 
        {
            GD.Print("Reloading...");
            WeaponMesh.SetVisible(false);
            this.TimeSinceReloaded = 0f;
        }
    }

    public void Spawn(Node camera, string Name)
    {
        MainNode = camera.GetNode("/root/Main");
        PackedScene PackedScene = (PackedScene)ResourceLoader.Load(WeaponResource);
        WeaponMesh = (MeshInstance)PackedScene.Instance();
        camera.AddChild(WeaponMesh);
        WeaponMesh.Translation = this.SpawnTranslation;
        WeaponMesh.SetName(Name);
        WeaponMesh.SetVisible(false);
        muzzleFlash = (Sprite3D)WeaponMesh.GetNode("MuzzleFlash");
        shootSound = (AudioStreamPlayer3D)WeaponMesh.GetNode("ShootSound");
        reloadSound = (AudioStreamPlayer3D)WeaponMesh.GetNode("ReloadSound");
        // projectile mesh
        if (Projectile)
        {
            ProjectileScene = (PackedScene)ResourceLoader.Load(ProjectileResource);
            GD.Print("loaded projectilescene");
            GD.Print(ProjectileScene.ResourceName);
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
        _weaponResource = "res://Shotgun.tscn";
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
        _weaponResource = "res://Shotgun.tscn";
    }
}

public class ConcussionGrenade : Weapon
{
    public ConcussionGrenade() {
        GD.Print("ConcussionGrenade");
        _damage = 0;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.ConcussionGrenade;
        _weaponResource = "res://Shotgun.tscn";
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
        _weaponResource = "res://Shotgun.tscn";
    }
}

public class NailGrenade : Weapon
{
    public NailGrenade() {
        GD.Print("NailGrenade");
        _damage = 50;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.NailGrenade;
        _weaponResource = "res://Shotgun.tscn";
    }
}

public class MIRVGrenade : Weapon
{
    public MIRVGrenade() {
        GD.Print("MIRVGrenade");
        _damage = 50;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.MIRVGrenade;
        _weaponResource = "res://Shotgun.tscn";
    }
}

public class NapalmGrenade : Weapon
{
    public NapalmGrenade() {
        GD.Print("NapalmGrenade");
        _damage = 20;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.NapalmGrenade;
        _weaponResource = "res://Shotgun.tscn";
    }
}

public class GasGrenade : Weapon
{
    public GasGrenade() {
        GD.Print("GasGrenade");
        _damage = 10;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.GasGrenade;
        _weaponResource = "res://Shotgun.tscn";
    }
}

public class EMPGrenade : Weapon
{
    public EMPGrenade() {
        GD.Print("EMPGrenade");
        _damage = 0;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.EMPGrenade;
        _weaponResource = "res://Shotgun.tscn";
    }
}

public class Axe : Weapon
{
    public Axe() {
        GD.Print("Axe");
        _damage = 25;
        _minAmmoRequired = 0;
        _ammoType = Ammunition.Axe;
        _weaponResource = "res://Shotgun.tscn";
    }
}

public class Shotgun : Weapon
{
    public Shotgun() {
        GD.Print("Shotgun");
        _damage = 25;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Shells;
        _weaponResource = "res://Shotgun.tscn";
    }
}

public class SuperShotgun : Weapon
{
    public SuperShotgun() {
        GD.Print("SuperShotgun");
        _damage = 50;
        _minAmmoRequired = 2;
        _ammoType = Ammunition.Shells;
        _weaponResource = "res://Shotgun.tscn";
    }
}

public class NailGun : Weapon
{
    public NailGun() {
        GD.Print("NailGun");
        _damage = 15;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Nails;
        _weaponResource = "res://MachineGun.tscn";
    }
}

public class SniperRifle : Weapon
{
    public SniperRifle() {
        GD.Print("SniperRifle");
        _damage = 10;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Shells;
        _weaponResource = "res://Shotgun.tscn";
    }
}

public class AutoRifle : Weapon
{
    public AutoRifle() {
        GD.Print("AutoRifle");
        _damage = 10;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Shells;
        _weaponResource = "res://Shotgun.tscn";
    }
}

public class SuperNailGun : Weapon
{
    public SuperNailGun() {
        GD.Print("SuperNailGun");
        _damage = 30;
        _minAmmoRequired = 2;
        _ammoType = Ammunition.Nails;
        _weaponResource = "res://Shotgun.tscn";
    }
}

public class GrenadeLauncher : Weapon
{
    public GrenadeLauncher() {
        GD.Print("GrenadeLauncher");
        _damage = 100;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Rockets;
        _weaponResource = "res://Shotgun.tscn";
    }
}

public class PipebombLauncher : Weapon
{
    public PipebombLauncher() {
        GD.Print("PipebombLauncher");
        _damage = 100;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Rockets;
        _weaponResource = "res://Shotgun.tscn";
    }
}

public class RocketLauncher : Weapon
{
    public RocketLauncher() {
        GD.Print("RocketLauncher");
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Rockets;
        _weaponResource = "res://Shotgun.tscn";
        _projectile = true;
        _projectileResource = "res://Rocket.tscn";
        _clipSize = 4;
        _clipLeft = _clipSize == -1 ? 999 : _clipSize;
    }
}

public class Syringe : Weapon
{
    public Syringe() {
        GD.Print("Syringe");
        _damage = 10;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Axe;
        _weaponResource = "res://Shotgun.tscn";
    }
}

public class MiniGun : Weapon
{
    public MiniGun() {
        GD.Print("MiniGun");
        _damage = 10;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Shells;
        _weaponResource = "res://Shotgun.tscn";
    }
}

public class FlameThrower : Weapon
{
    public FlameThrower() {
        GD.Print("FlameThrower");
        _damage = 10;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Cells;
        _weaponResource = "res://Shotgun.tscn";
    }
}

public class PyroLauncher : Weapon
{
    public PyroLauncher() {
        GD.Print("PyroLauncher");
        _damage = 10;
        _minAmmoRequired = 3;
        _ammoType = Ammunition.Rockets;
        _weaponResource = "res://Shotgun.tscn";
    }
}

public class Tranquiliser : Weapon
{
    public Tranquiliser() {
        GD.Print("Tranquiliser");
        _damage = 10;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Shells;
        _weaponResource = "res://Shotgun.tscn";
    }
}

public class Knife : Weapon
{
    public Knife() {
        GD.Print("Knife");
        _damage = 100;
        _minAmmoRequired = 0;
        _ammoType = Ammunition.Axe;
        _weaponResource = "res://Shotgun.tscn";
    }
}

public class RailGun : Weapon
{
    public RailGun() {
        GD.Print("RailGun");
        _damage = 20;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Nails;
        _weaponResource = "res://Shotgun.tscn";
    }
}

public class Spanner : Weapon
{
    public Spanner() {
        GD.Print("Spanner");
        _damage = 25;
        _minAmmoRequired = 0;
        _ammoType = Ammunition.Axe;
        _weaponResource = "res://Shotgun.tscn";
    }
}

