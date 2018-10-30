using Godot;
using System;
using System.Collections.Generic;



public class MIRVGrenade : HandGrenade
{
    public static string ProjectileResource = "res://Scenes/HandGrenades/MIRVGrenade.tscn";
    private static string MIRVResource = "res://Scenes/Weapons/Grenade.tscn";

    public MIRVGrenade()
    {
        _damage = 30;
    }

    override protected void PrimeTimeFinished()
    {
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

        // now have original mirv explode
        this.Explode(_damage);
    }
}