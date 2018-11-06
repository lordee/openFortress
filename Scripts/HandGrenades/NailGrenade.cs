using Godot;
using System;
using System.Collections.Generic;



public class NailGrenade : HandGrenade
{
    public static string ProjectileResource = "res://Scenes/HandGrenades/NailGrenade.tscn";
    private Vector3 _direction;
    private Vector3 _destination;
    private float _speed = 10;
    private float _rotationSpeed = 10;
    Spatial _nailSpawn;
    Vector3 _nailSpawnLocation = new Vector3(0,0,5);
    private float _lastFired;
    private float _nailCooldown = 0.5f;
    private string _projectileResource = "res://Scenes/Weapons/Nail.tscn";
    
    public NailGrenade()
    {
        _damage = 30;
        _stageOne = true;
    }

    override protected void PrimeTimeFinished()
    {
        if (_stageOne)
        {
            _stageOne = false;
            // set direction to rise from current location
            _direction = new Vector3(0,3,0);
            _destination = this.Translation + _direction;
            _direction = _direction.Normalized();
            // reset activetime for stage two
            _activeTime = 0f;
            // add spawn point on grenade for nails
            _nailSpawn = new Spatial();
            this.AddChild(_nailSpawn);
            _nailSpawn.Translation = this.Translation + _nailSpawnLocation;
        }
        else
        {
            // when lifetime ends, call explode
            this.Explode(_damage);
        }
    }

    override protected void StageTwoPhysicsProcess(float delta)
    {
        if (!_stageOne)
        {
            // grenade rises from current location
            if (_destination.y - this.Translation.y > 1f)
            {
                Vector3 motion = _direction * _speed * delta;
                this.MoveAndCollide(motion);
            }
            else
            {
                _lastFired += delta;
                // grenade rotates
                this.RotateY(Mathf.Deg2Rad(_rotationSpeed * delta));
                // once grenade at destination, grenade fires nails for lifetime
                if (_lastFired >= _nailCooldown)
                {
                    PackedScene nailScene = (PackedScene)ResourceLoader.Load(_projectileResource);
                    Projectile nail = (Projectile)nailScene.Instance();
                    _playerOwner.MainNode.AddChild(nail);
                    Transform t = _nailSpawn.GetGlobalTransform();
                    
                    nail.Init(t, t.origin + (_nailSpawnLocation * 1000), _playerOwner, null, "nailgrenadenail", 0, 40, 30);
                }
            }
        }
    }
}