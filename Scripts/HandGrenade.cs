using Godot;
using System;
using System.Collections.Generic;



abstract public class HandGrenade : KinematicBody
{
    private float _primedTime = 0;
    private float _lifeTime = 3.0f;
    private Player _shooter;
    private Camera _camera;
    private bool _explodeNextTick = false;
    private Vector3 _velocity;
    private Vector3 _direction = new Vector3();
    private float _currentSpeed = 20;
    private float _gravity = 1.0f;
    protected float _damage;
    private float _areaOfEffectRadius = 5f;

    private string _particleResource = "res://Scenes/Weapons/RocketExplosion.tscn";
    private PackedScene _particleScene;
    private PackedScene _projectileScene;

    protected Player _playerOwner;

    protected Ammunition _ammoType;
    protected string _projectileResource;
    public string ProjectileResource {
        get { return _projectileResource; }
    }
    private bool _thrown = false;

    public HandGrenade()
    {
    }

    public override void _PhysicsProcess(float delta)
    {
        if (_explodeNextTick)
        {
            this.Explode(_damage);
        }

        _primedTime += delta;
        if (_thrown)
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
    }

    public void Prime(Player pOwner, float damage)
    {
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
        GD.Print("exploding");
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
        foreach (Dictionary<object, object>  r in result) {
            if (r["collider"] is Player pl)
            {
                GD.Print("found player");
                // find how far from explosion as a percentage, apply to damage
                float dist = this.Transform.origin.DistanceTo(pl.Transform.origin);
                dist = dist > this._areaOfEffectRadius ? (this._areaOfEffectRadius*.99f) : dist;
                float pc = ((this._areaOfEffectRadius - dist) / this._areaOfEffectRadius);
                float d = damage * pc;
                GD.Print("dam: " + damage);
                GD.Print("pc: " + pc);
                GD.Print("dist: " + dist);
                GD.Print("inflicted dam: " + d);
                // inflict damage
                pl.TakeDamage(this.Transform, this.GetType().ToString().ToLower(), 0, this._playerOwner, d);
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
    public static string ProjectileResource = "res://Scenes/Weapons/Shotgun.tscn";
}

public static class ConcussionGrenade
{
    public static float Damage = 0;
    public static string ProjectileResource = "res://Scenes/Weapons/Shotgun.tscn";
}

// this is useless, replace it with something
public static class Flare
{
    public static float Damage = 0;
    public static string ProjectileResource = "res://Scenes/Weapons/Shotgun.tscn";
}

public static class NailGrenade
{
    public static float Damage = 0;
    public static string ProjectileResource = "res://Scenes/Weapons/Shotgun.tscn";
}

public static class MIRVGrenade
{
    public static float Damage = 30;
    public static string ProjectileResource = "res://Scenes/Weapons/Shotgun.tscn";
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