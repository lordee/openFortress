using Godot;
using System;

public class FragGrenadeO : Projectile
{
    private float _gravity = 1.0f;
    private float _time = 0f;
    public float Time {
        get { return _time; }
        set { _time = value; }
    }
    protected float _lifeTime = 3.0f;
    private Vector3 _velocity;

    public override void _Ready()
    {
        _particleResource = "res://Scenes/Weapons/RocketExplosion.tscn";
        _areaOfEffectRadius = 5;
        _areaOfEffect = true;
    }

    public override void _PhysicsProcess(float delta)
    {
        _time += delta;        
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
        // after 3 seconds, explode
        if (_time > _lifeTime)
        {
            this.Explode(null, _damage);
        }
    }


}
