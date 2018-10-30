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
    protected PackedScene _particleScene;
    protected bool _areaOfEffect;
    protected float _areaOfEffectRadius;
    protected Player _playerOwner;
    public Weapon WeaponOwner;
    protected string _weaponOwnerString;
    private float _weaponOwnerInflictLength;
    protected float _currentSpeed;

    

    public Projectile()
    {
    }

    public void Init(Transform t, Vector3 aimAt, Player pOwner, Weapon weaponOwner, string weaponOwnerString, float weaponOwnerInflictLength, int speed, float damage)
    {   
        this.AddCollisionExceptionWith(pOwner);
        this.Transform = t;
        _particleScene = (PackedScene)ResourceLoader.Load(_particleResource);
        _playerOwner = pOwner;
        WeaponOwner = weaponOwner;
        _weaponOwnerString = weaponOwnerString;
        _weaponOwnerInflictLength = weaponOwnerInflictLength;
        _speed = speed;
        _currentSpeed = _speed;
        _damage = damage;
        _direction = aimAt;
        _direction = _direction.Normalized();
    }

    public override void _PhysicsProcess(float delta)
    {
        Vector3 vel = _direction * _speed;
        Vector3 motion = vel * delta;
        KinematicCollision c = this.MoveAndCollide(motion);
        if (c != null)
        {
            Random ran = new Random();
            float damage = _damage + ran.Next(0,20);
            // if c collider is kinematic body (direct hit)
            if (c.Collider is Player pl)
            {
                pl.TakeDamage(this.Transform, _weaponOwnerString, _weaponOwnerInflictLength, _playerOwner, damage);
                this.Explode(pl, damage);
            }
            else {
                this.Explode(null, damage);
            }
        }
    }

    public void Explode(Player ignore, float damage)
    {
        if (_areaOfEffect)
        {
            this.FindRadius(ignore, damage);
        }
        
        Particles p = (Particles)_particleScene.Instance();
        p.Transform = this.Transform;
        GetNode("/root/OpenFortress/Main").AddChild(p);
        p.Emitting = true;
        
        // remove projectile
        GetTree().QueueDelete(this);
    }

    protected void FindRadius(Player ignore, float damage)
    {
        // test for radius damage
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
                if (pl != ignore || ignore == null)
                {
                    // find how far from explosion as a percentage, apply to damage
                    float dist = this.Transform.origin.DistanceTo(pl.Transform.origin);
                    dist = dist > this._areaOfEffectRadius ? (this._areaOfEffectRadius*.99f) : dist;
                    float pc = ((this._areaOfEffectRadius - dist) / this._areaOfEffectRadius);
                    float d = damage * pc;
                    GD.Print("dam: " + damage);
                    GD.Print("pc: " + pc);
                    GD.Print("dist: " + dist);
                    GD.Print("aoe: " + this._areaOfEffectRadius);
                    GD.Print(this._playerOwner.GetName());
                    GD.Print(pl.GetName());
                    if (pl == this._playerOwner)
                    {
                        d = d * 0.5f;
                    }
                    GD.Print("inflicted dam: " + d);
                    // inflict damage
                    pl.TakeDamage(this.Transform, _weaponOwnerString, _weaponOwnerInflictLength, this._playerOwner, d);
                }
            }
        }
    }
}