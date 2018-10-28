using Godot;
using System;
using System.Collections.Generic;



abstract public class HandGrenade : KinematicBody
{
    private float _primedTime = 0;
    private float _lifeTime = 3.0f;
    private bool _explodeNextTick = false;
    private Vector3 _velocity;
    private Vector3 _direction = new Vector3();
    private float _currentSpeed = 20;
    private float _gravity = 1.0f;
    private float _damage;
    private float _areaOfEffectRadius = 5f;
    private string _particleResource = "res://Scenes/Weapons/RocketExplosion.tscn";
    private PackedScene _particleScene;
    private Player _playerOwner;
    private Ammunition _grenadeType;
    private bool _thrown = false;

    public HandGrenade()
    {
    }

    public override void _PhysicsProcess(float delta)
    {
        _primedTime += delta;

        // let grenade "drop" for a frame
        if (_explodeNextTick)
        {
            this.Explode(_damage);
        }
        
        // after 3 seconds, explode
        if (_primedTime > _lifeTime)
        {
            if (_thrown)
            {
                this.Explode(_damage);
            }
            else
            {
                this.Transform = this._playerOwner.GetGlobalTransform();
                _explodeNextTick = true;
            }
        }

        if (_thrown || _explodeNextTick)
        {
            _velocity = _direction * _currentSpeed;  
            Vector3 motion = _velocity * delta;

            KinematicCollision c = this.MoveAndCollide(motion);

            if (c != null)
            {
                // bounce
                _direction = motion.Bounce(c.Normal);
                _currentSpeed *= .95f;
            }
            else {
                // apply gravity
                _direction.y -= _gravity * delta;
            }
        }       
    }

    public void Prime(Player pOwner, float damage, Ammunition grenadeType)
    {
        _grenadeType = grenadeType;
        _damage = damage;
        _particleScene = (PackedScene)ResourceLoader.Load(_particleResource);
        this.AddCollisionExceptionWith(pOwner);
        _playerOwner = pOwner;
        this.Visible = false;
    }

    public void Throw(Transform t)
    {
        this.Transform = t;
        Vector3 init = new Vector3();
        init -= this.Transform.basis.z;
        // spawn it in front of player
        this.SetTranslation(this.GetTranslation() + init);
        _direction -= this.Transform.basis.z;
        _direction = _direction.Normalized();
        _thrown = true;
        this.Visible = true;
    }

    public void Explode(float damage)
    {       
        object[] result = this.FindPlayersInRadius();
        switch (_grenadeType)
        {
            case Ammunition.MIRVGrenade:
                // spawn child grenades
                PackedScene _projectileScene = (PackedScene)ResourceLoader.Load(MIRVGrenade.MIRVResource);
                Grenade[] mirvs = new Grenade[4];
                Random ran = new Random();
                for (int i = 0; i < 4; i++)
                {
                    // spawn projectile, set it moving
                    Grenade _projectileMesh = (Grenade)_projectileScene.Instance();
                    
                    // add to scene
                    GetNode("/root/OpenFortress/Main").AddChild(_projectileMesh);
                    Vector3 dir = new Vector3(ran.Next(150), ran.Next(150), ran.Next(150));
                    _projectileMesh.MIRVInit(this.GetGlobalTransform(), _playerOwner, "mirvgrenade", 20, 100, dir);
                    mirvs[i] = _projectileMesh;
                }

                // make sure they don't just bounce on each other and float in air
                for (int i = 0; i < 4; i++)
                {
                    for (int i2 = 0; i2 < 4; i2++)
                    {
                        if (i2 != i)
                        {
                            mirvs[i].AddCollisionExceptionWith(mirvs[i2]);
                        }
                    }
                }
            break;
        }

        foreach (Dictionary<object, object>  r in result) {
            if (r["collider"] is Player pl)
            {
                GD.Print("found player");
                // find how far from explosion as a percentage
                float dist = this.Transform.origin.DistanceTo(pl.Transform.origin);
                dist = dist > this._areaOfEffectRadius ? (this._areaOfEffectRadius*.99f) : dist;
                float pc = ((this._areaOfEffectRadius - dist) / this._areaOfEffectRadius);
                GD.Print("dam: " + damage);
                GD.Print("pc: " + pc);
                GD.Print("dist: " + dist);
                
                switch (_grenadeType)
                {
                    case Ammunition.MIRVGrenade:
                    case Ammunition.FragGrenade:
                        // apply percentage to damage
                        float d = damage * pc;
                        GD.Print("inflicted dam: " + d);
                        // inflict damage
                        pl.TakeDamage(this.Transform, this.GetType().ToString().ToLower(), 0, this._playerOwner, d);
                    break;
                    case Ammunition.MFTGrenade:
                        pl.AddVelocity(this.Transform.origin, ConcussionGrenade.BlastPower * (1 - pc));
                    break;
                    case Ammunition.ConcussionGrenade:
                        pl.Inflict("concussiongrenade", ConcussionGrenade.InflictLength, _playerOwner);
                        pl.AddVelocity(this.Transform.origin, ConcussionGrenade.BlastPower * (1 - pc));
                    break;
                }
            }
        }
        
        Particles p = (Particles)_particleScene.Instance();
        p.Transform = this.GetGlobalTransform();
        GetNode("/root/OpenFortress/Main").AddChild(p);
        p.Emitting = true;
        
        // remove projectile
        _playerOwner.PrimedGrenade = null;
        GetTree().QueueDelete(this);
    }

    private object[] FindPlayersInRadius()
    {
        SphereShape s = new SphereShape();
        s.SetRadius(_areaOfEffectRadius);

        // Get space and state of the subject body
        RID space = PhysicsServer.BodyGetSpace(this.GetRid());
        PhysicsDirectSpaceState state = PhysicsServer.SpaceGetDirectState(space);

        // Setup shape query parameters
        PhysicsShapeQueryParameters par = new PhysicsShapeQueryParameters();
        par.SetShapeRid(s.GetRid());
        par.Transform = this.Transform;
        
        object[] result = state.IntersectShape(par);

        return result;
    }
}

public static class FragGrenade
{
    public static float Damage = 100;
    public static string ProjectileResource = "res://Scenes/Weapons/FragGrenade.tscn"; 
}

// magic fly tool grenade - old concussion
public static class MFTGrenade
{
    public static float Damage = 0;
    public static float BlastPower = 30;
    public static string ProjectileResource = "res://Scenes/Weapons/FragGrenade.tscn";
}

public static class ConcussionGrenade
{
    public static float Damage = 0;
    public static float BlastPower = 30;
    public static string ProjectileResource = "res://Scenes/Weapons/FragGrenade.tscn";
    public static float InflictLength = 10;
}

public static class NailGrenade
{
    public static float Damage = 0;
    public static string ProjectileResource = "res://Scenes/Weapons/Shotgun.tscn";
}

public static class MIRVGrenade
{
    public static float Damage = 30;
    public static string ProjectileResource = "res://Scenes/Weapons/FragGrenade.tscn";
    public static string MIRVResource = "res://Scenes/Weapons/Grenade.tscn";
}

public static class NapalmGrenade
{
    public static float Damage = 20;
    public static string ProjectileResource = "res://Scenes/Weapons/Shotgun.tscn";
}

public static class GasGrenade
{
    public static float Damage = 0;
    public static string ProjectileResource = "res://Scenes/Weapons/Shotgun.tscn";
}

public static class EMPGrenade
{
    public static float Damage = 0;
    public static string ProjectileResource = "res://Scenes/Weapons/Shotgun.tscn";
}