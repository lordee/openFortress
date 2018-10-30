using Godot;
using System;
using System.Collections.Generic;



abstract public class HandGrenade : KinematicBody
{
    protected float _activeTime = 0;
    private float _maxPrimedTime = 3.0f;

    private bool _explodeNextTick = false;
    private Vector3 _velocity;
    private Vector3 _direction = new Vector3();
    private float _currentSpeed = 20;
    private float _gravity = 1.0f;
    protected float _damage = 0;
    protected float _inflictLength = 0;
    protected float _areaOfEffectRadius = 5f;
    private string _particleResource = "res://Scenes/Weapons/RocketExplosion.tscn";
    private PackedScene _particleScene;
    protected Player _playerOwner;
    private bool _thrown = false;
    // used by nail grenade to indicate if it should be getting affected by gravity, shooting nails etc
    protected bool _stageOne = true;

    public HandGrenade()
    {
    }

    public override void _PhysicsProcess(float delta)
    {
        _activeTime += delta;
        StageTwoPhysicsProcess(delta);
        // let grenade "drop" for a frame
        if (_explodeNextTick)
        {
            this.PrimeTimeFinished();
        }
        
        // after 3 seconds, explode
        if (_activeTime > _maxPrimedTime)
        {
            if (_thrown)
            {
                this.PrimeTimeFinished();
            }
            else
            {
                this.Transform = this._playerOwner.GetGlobalTransform();
                _explodeNextTick = true;
            }
        }

        if ((_thrown || _explodeNextTick) && _stageOne)
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

    virtual protected void StageTwoPhysicsProcess(float delta)
    {
    }

    public void Prime(Player pOwner)
    {
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

    virtual protected void PrimeTimeFinished()
    {
        this.Explode(_damage);
    }

    virtual public void Explode(float val)
    {       
        object[] result = this.FindPlayersInRadius();

        foreach (Dictionary<object, object>  r in result) {
            if (r["collider"] is Player pl)
            {
                // find how far from explosion as a percentage
                float dist = this.Transform.origin.DistanceTo(pl.Transform.origin);
                dist = dist > this._areaOfEffectRadius ? (this._areaOfEffectRadius*.99f) : dist;
                float pc = ((this._areaOfEffectRadius - dist) / this._areaOfEffectRadius);

                // apply percentage to damage
                float d = val * pc;
                // inflict damage
                pl.TakeDamage(this.Transform, this.GetType().ToString().ToLower(), 0, this._playerOwner, d);
            }
        }
        
        this.FinishExplode();
    }

    protected void FinishExplode()
    {
        Particles p = (Particles)_particleScene.Instance();
        p.Transform = this.GetGlobalTransform();
        GetNode("/root/OpenFortress/Main").AddChild(p);
        p.Emitting = true;
        
        // remove projectile
        _playerOwner.PrimedGrenade = null;
        GetTree().QueueDelete(this);
    }

    protected object[] FindPlayersInRadius()
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