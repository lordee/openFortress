using Godot;
using System;
using System.Collections.Generic;

public class Projectile : KinematicBody
{
    protected Vector3 _direction = new Vector3();
    protected Vector3 _up = new Vector3(0,1,0);
    protected int _speed;
    protected float _damage;
    protected string _particleResource;
    private PackedScene _particleScene;
    protected bool _areaOfEffect;
    protected float _areaOfEffectRadius;
    protected string _projectileType;
    private Player _playerOwner;

    private bool AreaOfEffect {
        get { return _areaOfEffect; }
        set { _areaOfEffect = value; }
    }

    private float AreaOfEffectRadius {
        get { return _areaOfEffectRadius; }
        set { _areaOfEffectRadius = value; }
    }

    private int Speed {
        get { return _speed; }
        set { _speed = value; }
    }
    private float Damage {
        get { return _damage; }
        set { _damage = value; }
    }
    private string ParticleResource {
        get { return _particleResource; }
        set { _particleResource = value; }
    }

    private string ProjectileType {
        get { return _projectileType; }
        set {_projectileType = value; }
    }

    public Projectile()
    {
    }

    public void Init(Transform t, Player pOwner)
    {       
        this.Transform = t;
        Vector3 init = new Vector3();
        init -= this.Transform.basis.z;
        // spawn it in front of player
        this.SetTranslation(this.GetTranslation() + init);
        _particleScene = (PackedScene)ResourceLoader.Load(ParticleResource);
        _playerOwner = pOwner;
    }

    public override void _PhysicsProcess(float delta)
    {
        _direction -= this.Transform.basis.z;
        _direction = _direction.Normalized();
        Vector3 vel = _direction * Speed;
        Vector3 motion = vel * delta;
        KinematicCollision c = this.MoveAndCollide(motion);
        Random ran = new Random();
        float damage = this.Damage + ran.Next(0,20);

        if (c != null)
        {
            // if c collider is kinematic body (direct hit)
            if (c.Collider is Player pl)
            {
                if (pl != this._playerOwner)
                {
                    GD.Print("direct hit");
                    GD.Print(pl.GetName());
                    GD.Print(this._playerOwner.GetName());
                    
                    pl.TakeDamage(this.Transform, this.ProjectileType, this._playerOwner, damage);

                    this.FindRadius(pl, damage);
                    this.Explode();
                }
            }
            else {
                
                this.FindRadius(null, damage);
                this.Explode();
            }
        }
    }

    private void Explode()
    {
        Particles p = (Particles)_particleScene.Instance();
        p.Transform = this.Transform;
        GetNode("/root/Main").AddChild(p);
        p.Emitting = true;
        
        // remove projectile
        GetTree().QueueDelete(this);
    }

    private void FindRadius(Player ignore, float damage)
    {

        // test for radius damage
        SphereShape s = new SphereShape();
        s.SetRadius(this.AreaOfEffectRadius);

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
                if (pl != ignore || ignore == null)
                {
                    // find how far from explosion as a percentage, apply to damage
                    float dist = this.Transform.origin.DistanceTo(pl.Transform.origin);
                    dist = dist > this.AreaOfEffectRadius ? (this.AreaOfEffectRadius*.99f) : dist;
                    float pc = ((this.AreaOfEffectRadius - dist) / this.AreaOfEffectRadius);
                    float d = damage * pc;
                    GD.Print("dam: " + damage);
                    GD.Print("pc: " + pc);
                    GD.Print("dist: " + dist);
                    GD.Print("aoe: " + this.AreaOfEffectRadius);
                    GD.Print(this._playerOwner.GetName());
                    GD.Print(pl.GetName());
                    if (pl == this._playerOwner)
                    {
                        d = d * 0.5f;
                    }
                    GD.Print("inflicted dam: " + d);
                    // inflict damage
                    pl.TakeDamage(this.Transform, this.ProjectileType, this._playerOwner, d);
                }
            }
        }
    }
}

public class Rocket : Projectile
{
    public override void _Ready()
    {
        _speed = 25;
        _damage = 100;
        _particleResource = "res://Scenes/Weapons/RocketExplosion.tscn";
        _areaOfEffect = true;
        _areaOfEffectRadius = 5;
        _projectileType = "rocket";
    }
}
