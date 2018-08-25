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

public enum WeaponType
{
    Melee,
    Spread,
    Hitscan,
    Projectile,
    Grenade
}

public enum PuffType
{
    Blood,
    Puff
}

abstract public class Weapon : MeshInstance
{
    protected int _damage;
    public int Damage 
    {
        get { return _damage; }
    }
    protected string _weaponResource;
    protected MeshInstance _weaponMesh;
    protected string _scene;
    protected int _minAmmoRequired;
    protected Ammunition _ammoType;
    protected int _clipSize;
    protected int _clipLeft;
    protected float _timeSinceLastShot;
    protected float _coolDown;
    protected float _timeSinceReloaded;
    protected float _reloadTime;
    protected string _projectileResource;
    protected Projectile _projectileMesh;
    protected PackedScene _projectileScene;   
    protected int _projectileSpeed;
    protected WeaponType _weaponType;
    protected float _shootRange = 0f;
    protected float _inflictLength = 0f;
    public float InflictLength {
        get { return _inflictLength; }
    }
    
    protected Vector3 _spread; 
    protected float _pelletCount = 1;
    
    private Sprite3D muzzleFlash;
    private AudioStreamPlayer3D shootSound;
    private AudioStreamPlayer3D reloadSound;

    string puffResource = "res://Scenes/Weapons/Puff.tscn";
    string bloodResource = "res://Scenes/Weapons/BloodPuff.tscn";
    PackedScene puffScene;
    PackedScene bloodScene;
    
    Node MainNode;

    public Weapon() {
        puffScene = (PackedScene)ResourceLoader.Load(puffResource);
        bloodScene = (PackedScene)ResourceLoader.Load(bloodResource);
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
    public Ammunition AmmoType {
        get { return _ammoType; }
    }
    public int MinAmmoRequired {
        get { return _minAmmoRequired; }
    }

    public MeshInstance WeaponMesh {
        get { return _weaponMesh; }
    }

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

    public bool Shoot(Camera camera, Vector2 cameraCenter, Player shooter) 
    {
        bool shot = false;
        // if enough ammunition in clip
        if (ClipLeft >= _minAmmoRequired)
        {
            // if weapon has hit cooldown
            if (!shooter.Tranquilised && TimeSinceLastShot >= _coolDown
                || shooter.Tranquilised && TimeSinceLastShot >= _coolDown * 2)
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
                
                switch (_weaponType)
                {
                    case WeaponType.Hitscan:
                    case WeaponType.Melee:
                    case WeaponType.Spread:
                        List<Tuple<Vector3, PuffType, Node>> puffList = new List<Tuple<Vector3, PuffType, Node>>();
                        Dictionary<KinematicBody, float> hitList = new Dictionary<KinematicBody, float>();
                        PhysicsDirectSpaceState spaceState = shooter.GetWorld().DirectSpaceState;
                        Vector3 shootOrigin = camera.ProjectRayOrigin(new Vector2(cameraCenter.x, cameraCenter.y));
                        Vector3 shootTo = camera.ProjectRayNormal(new Vector2(cameraCenter.x, cameraCenter.y)) * _shootRange;
                        
                        if (_weaponType == WeaponType.Spread)
                        {
                            float pc = _pelletCount;
                            Random ran = new Random();
                            float random = 0f;
                            while (pc > 0)
                            {
                                random = (float)ran.Next(0,100);
                                Vector3 newTo = new Vector3((shootOrigin.x + shootTo.x) + random * _spread.x, (shootOrigin.y + shootTo.y) + random * _spread.y, shootOrigin.z + shootTo.z);
                                GD.Print("random: " + random);
                                GD.Print("dir: " + newTo);

                                Tuple<Vector3, PuffType, Node, KinematicBody, float> data = this.DoHit(spaceState, shootOrigin, newTo, shooter);
                                if (data != null)
                                {
                                    if (data.Item1 != null)
                                    {
                                        puffList.Add(new Tuple<Vector3, PuffType, Node>(data.Item1, data.Item2, data.Item3));
                                    }
                                    
                                    if (data.Item4 != null)
                                    {
                                        hitList.Add(data.Item4, data.Item5);
                                    }
                                }
                                
                                pc -= 1;
                            }
                        }
                        else
                        {
                            Vector3 newTo = shootTo + shootOrigin;
                            Tuple<Vector3, PuffType, Node, KinematicBody, float> data = this.DoHit(spaceState, shootOrigin, newTo, shooter);

                            if (data != null)
                            {
                                if (data.Item1 != null)
                                {
                                    puffList.Add(new Tuple<Vector3, PuffType, Node>(data.Item1, data.Item2, data.Item3));
                                }
                                
                                if (data.Item4 != null)
                                {
                                    hitList.Add(data.Item4, data.Item5);
                                }
                            }
                        }

                        // apply damage
                        foreach(KinematicBody kb in hitList.Keys)
                        {
                            Player hit = (Player)kb;
                            if (hit.TeamID == shooter.TeamID)
                            {
                                if (this.GetType().ToString().ToLower() == "syringe"
                                || this.GetType().ToString().ToLower() == "spanner")
                                {
                                    hit.Heal(shooter, this);
                                }
                            }
                            else
                            {
                                hit.TakeDamage(shooter.Transform, this, shooter, hitList[kb]);
                            }
                        }

                        // do puff particles and blood particles
                        foreach (Tuple<Vector3, PuffType, Node> puff in puffList)
                        {
                            Particles puffPart = null;
                            switch (puff.Item2)
                            {
                                case PuffType.Blood:
                                    puffPart = (Particles)bloodScene.Instance();
                                break;
                                case PuffType.Puff:
                                    puffPart = (Particles)puffScene.Instance();
                                break;
                            }

                            puffPart.SetTranslation(puff.Item1);
                            if (puff.Item3 != null)
                            {
                                puff.Item3.AddChild(puffPart);
                            }
                            else
                            {
                                _weaponMesh.GetNode("/root/Main").AddChild(puffPart);
                            }
                            
                            puffPart.Emitting = true;
                        }
                    break;
                    case WeaponType.Projectile:
                    case WeaponType.Grenade:
                        // spawn projectile, set it moving
                        _projectileMesh = (Projectile)_projectileScene.Instance();
                        
                        // add to scene
                        _weaponMesh.GetNode("/root/Main").AddChild(_projectileMesh);
                        
                        Transform t = camera.GetGlobalTransform();
                        _projectileMesh.Init(t, shooter, this, _projectileSpeed, _damage);

                        if (_projectileMesh is Pipebomb p)
                        {
                            // track against player
                            shooter.AddActivePipebomb(p);
                        }
                    break;
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

    private Tuple<Vector3, PuffType, Node, KinematicBody, float> DoHit(PhysicsDirectSpaceState spaceState, Vector3 shootOrigin, Vector3 shootTo, Player shooter)
    {
        Tuple<Vector3, PuffType, Node, KinematicBody, float> hitData = null;
        Dictionary<object, object> res = spaceState.IntersectRay(shootOrigin, shootTo, new object[] { this, shooter }, 1);
        if (res.Count > 0)
        {
            Vector3 pos = (Vector3)res["position"];
            PuffType puff;
            Node colNode = null;
            KinematicBody hit = null;
            float dam = 0;
            // track if collides, track puff counts
            // leave this for fun
            if (res["collider"] is RigidBody rb)
            {           
                Vector3 impulse = (pos - shooter.GlobalTransform.origin).Normalized();
                Vector3 position = pos - rb.GlobalTransform.origin;
                rb.ApplyImpulse(position, impulse * 10);

                puff = PuffType.Puff;
                colNode = (Node)rb;
            } 
            else if (res["collider"] is KinematicBody kb)
            {
                hit = kb;
                dam = _damage / _pelletCount;
                puff = PuffType.Blood;
                colNode = (Node)kb;
            }
            else
            {
                puff = PuffType.Puff;
            }
            
            hitData = new Tuple<Vector3, PuffType, Node, KinematicBody, float>(pos, puff, colNode, hit, dam);
        }

        return hitData;
    }

    public void Reload(bool reloadFinished)
    {
        if (reloadFinished)
        {
            GD.Print("Reloaded");
            _weaponMesh.SetVisible(true);
            _clipLeft = this.AmmoLeft < _clipSize ? this.AmmoLeft : _clipSize;
            this.Reloading = false;
        } 
        else if (!this.Reloading)
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
        if (_weaponType == WeaponType.Projectile || _weaponType == WeaponType.Grenade)
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
        _weaponType = WeaponType.Melee;
        _shootRange = 10f;
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
        _weaponType = WeaponType.Spread;
        _shootRange = 100f;
        _pelletCount = 6;
        _spread = new Vector3(.04f, .04f, 0f);
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
        _weaponType = WeaponType.Spread;
        _shootRange = 100f;
        _pelletCount = 14;
        _spread = new Vector3(.14f, .08f, 0f);
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
        _projectileResource = "res://Scenes/Weapons/Nail.tscn";
        _projectileSpeed = 40;
        _clipSize = -1;
        _clipLeft = _clipSize == -1 ? 999 : _clipSize;
        _coolDown = 0.2f;
        _weaponType = WeaponType.Projectile;
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
        _weaponType = WeaponType.Hitscan;
        _shootRange = 100f;
        _clipSize = 1;
        _reloadTime = 2f;
        _coolDown = 0f;
        _clipLeft = _clipSize == -1 ? 999 : _clipSize;
    }
}

public class SuperNailGun : NailGun
{
    public SuperNailGun() {
        GD.Print("SuperNailGun");
        _damage = 30;
        _minAmmoRequired = 2;
        _weaponResource = "res://Scenes/Weapons/SuperNailGun.tscn";
        _weaponType = WeaponType.Projectile;
    }
}

public class GrenadeLauncher : Weapon
{
    public GrenadeLauncher() {
        GD.Print("GrenadeLauncher");
        _damage = 120;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Rockets;
        _weaponResource = "res://Scenes/Weapons/GrenadeLauncher.tscn";
        _projectileResource = "res://Scenes/Weapons/Grenade.tscn";
        _projectileSpeed = 40;
        _weaponType = WeaponType.Grenade;
        _clipSize = 6;
        _coolDown = 0.6f;
        _reloadTime = 4.0f;
        _clipLeft = _clipSize == -1 ? 999 : _clipSize;
    }
}

public class PipebombLauncher : Weapon
{
    public PipebombLauncher() {
        GD.Print("PipebombLauncher");
        _damage = 120;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Rockets;
        _weaponResource = "res://Scenes/Weapons/PipebombLauncher.tscn";
        _projectileResource = "res://Scenes/Weapons/Pipebomb.tscn";
        _projectileSpeed = 40;
        _weaponType = WeaponType.Grenade;
        _clipSize = 6;
        _coolDown = 0.6f;
        _reloadTime = 4.0f;
        _clipLeft = _clipSize == -1 ? 999 : _clipSize;
    }
}

public class RocketLauncher : Weapon
{
    public RocketLauncher() {
        GD.Print("RocketLauncher");
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Rockets;
        _weaponResource = "res://Scenes/Weapons/RocketLauncher.tscn";
        _projectileResource = "res://Scenes/Weapons/Rocket.tscn";
        _projectileSpeed = 40;
        _damage = 100;
        _clipSize = 4;
        _clipLeft = _clipSize == -1 ? 999 : _clipSize;
        _coolDown = 1.0f;
        _reloadTime = 4.0f;
        _weaponType = WeaponType.Projectile;
    }
}

public class Syringe : Axe
{
    public Syringe() {
        GD.Print("Syringe");
        _damage = 10;
        _weaponResource = "res://Scenes/Weapons/Syringe.tscn";
        _inflictLength = 1f;
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
        _weaponType = WeaponType.Spread;
        _shootRange = 100f;
        _coolDown = 0.1f;
        _clipSize = -1;
        _clipLeft = _clipSize == -1 ? 999 : _clipSize;
        _pelletCount = 14;
        _spread = new Vector3(.14f, .08f, 0f);
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
        _weaponType = WeaponType.Projectile;
        _shootRange = 10f;
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
        _weaponType = WeaponType.Projectile;
    }
}

public class Tranquiliser : Weapon
{
    public Tranquiliser() {
        GD.Print("Tranquiliser");
        _damage = 25;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Shells;
        _weaponResource = "res://Scenes/Weapons/Tranquiliser.tscn";
        _weaponType = WeaponType.Projectile;
        _projectileResource = "res://Scenes/Weapons/Nail.tscn";
        _projectileSpeed = 40;
        _clipSize = -1;
        _clipLeft = _clipSize == -1 ? 999 : _clipSize;
        _coolDown = 2.0f;
        _inflictLength = 4.0f;
    }
}

public class Knife : Axe
{
    public Knife() {
        GD.Print("Knife");
        _damage = 100;
        _weaponResource = "res://Scenes/Weapons/Knife.tscn";
    }
}

public class RailGun : Weapon
{
    public RailGun() {
        GD.Print("RailGun");
        _damage = 40;
        _minAmmoRequired = 1;
        _ammoType = Ammunition.Nails;
        _weaponResource = "res://Scenes/Weapons/RailGun.tscn";
        _weaponType = WeaponType.Projectile;

        _ammoType = Ammunition.Nails;
        _projectileResource = "res://Scenes/Weapons/Nail.tscn";
        _projectileSpeed = 60;
        _clipSize = -1;
        _clipLeft = _clipSize == -1 ? 999 : _clipSize;
        _coolDown = .5f;
    }
}

public class Spanner : Axe
{
    public Spanner() {
        GD.Print("Spanner");
        _damage = 25;
        _weaponResource = "res://Scenes/Weapons/Spanner.tscn";
        _ammoType = Ammunition.Cells;
    }
}

