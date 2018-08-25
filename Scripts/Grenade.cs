using Godot;
using System;

public class Grenade : Projectile
{
    private float _gravity = 1.0f;
    private float _time = 0f;
    protected float _lifeTime = 2.5f;
    private Vector3 _velocity;
    
    public override void _Ready()
    {
        _particleResource = "res://Scenes/Weapons/RocketExplosion.tscn";
        _areaOfEffect = true;
        _areaOfEffectRadius = 5;
    }

    public override void _PhysicsProcess(float delta)
    {
        _time += delta;        
        _velocity = _direction * _currentSpeed;  
        Vector3 motion = _velocity * delta;

        KinematicCollision c = this.MoveAndCollide(motion);

        if (c != null)
        {
            // if c collider is kinematic body (direct hit)
            if (c.Collider is Player pl)
            {
                if (pl != this._playerOwner)
                {
                    pl.TakeDamage(this.Transform, this.WeaponOwner, _playerOwner, _damage);

                    this.Explode(pl, _damage);
                }
            }
            else {
                // bounce
                _direction = motion.Bounce(c.Normal);
                _currentSpeed *= .95f;
            }
        }
        else {
            // apply gravity
            _direction.y -= _gravity * delta;
        }
        // after 2.5 seconds, explode
        if (_time > _lifeTime)
        {
            this.Explode(null, _damage);
        }
    }
}
