using Godot;
using System;

public class Projectile : MeshInstance
{
    protected Vector3 _direction = new Vector3();
    protected Vector3 _up = new Vector3(0,1,0);
    protected int _speed;

    public int Speed {
        get { return _speed; }
        set { _speed = value; }
    }

    public Projectile()
    {
    }

    public void Init(Transform t)
    {       
        this.Transform = t;
    }
}

public class Rocket : Projectile
{
    // kinematic
    //KinematicBody kb;
    
    public int Damage = 100;

    public override void _Ready()
    {
        //kb = (KinematicBody)GetNode("KinematicBody");
        _speed = 5;
    }

    public override void _Process(float delta)
    {
        _direction -= this.Transform.basis.z;

        _direction = _direction.Normalized();
        Vector3 vel = _direction * Speed;
        Vector3 motion = vel * delta;
        this.SetTranslation(this.GetTranslation() + motion);
    }
}
