using Godot;
using System;

public class Projectile : KinematicBody
{
    protected Vector3 _direction = new Vector3();
    protected Vector3 _up = new Vector3(0,1,0);
    protected int _speed;
    protected int _damage;
    protected string _particleResource;
    private PackedScene _particleScene;
    protected bool _areaOfEffect;
    protected float _areaOfEffectRadius;

    public bool AreaOfEffect {
        get { return _areaOfEffect; }
        set { _areaOfEffect = value; }
    }

    public float AreaOfEffectRadius {
        get { return _areaOfEffectRadius; }
        set { _areaOfEffectRadius = value; }
    }

    public int Speed {
        get { return _speed; }
        set { _speed = value; }
    }
    public int Damage {
        get { return _damage; }
        set { _damage = value; }
    }
    private string ParticleResource {
        get { return _particleResource; }
        set { _particleResource = value; }
    }

    public Projectile()
    {
    }

    public void Init(Transform t)
    {       
        this.Transform = t;
        Vector3 init = new Vector3();
        init -= this.Transform.basis.z;
        // spawn it in front of player
        this.SetTranslation(this.GetTranslation() + init);
        _particleScene = (PackedScene)ResourceLoader.Load(ParticleResource);
    }

    public override void _PhysicsProcess(float delta)
    {
        _direction -= this.Transform.basis.z;

        _direction = _direction.Normalized();
        Vector3 vel = _direction * Speed;
        Vector3 motion = vel * delta;
        KinematicCollision c = this.MoveAndCollide(motion);
        
        if (c != null)
        {
            // spawn explosion
            Particles p = (Particles)_particleScene.Instance();
            p.Transform = this.Transform;
            GetNode("/root/Main").AddChild(p);
            p.Emitting = true;
            
            // remove projectile
            GetTree().QueueDelete(this);

            // apply damage?
            // if c collider is kinematic body or if there are kinematic bodies nearby
            // have them take damage
            // taking damage causes movement to be added
            GD.Print(c.Collider);
        }
    }
}

public class Rocket : Projectile
{
    public override void _Ready()
    {
        _speed = 5;
        _damage = 100;
        _particleResource = "res://Scenes/Weapons/RocketExplosion.tscn";
        _areaOfEffect = true;
        _areaOfEffectRadius = 10f;
    }
}
