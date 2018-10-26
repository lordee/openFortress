using Godot;
using System;
using System.Collections.Generic;



public class HandGrenadeManager
{
    HandGrenade _primedGrenade1;
    public HandGrenade PrimedGrenade1 {
        get { return _primedGrenade1; }
    }
    private string _primedGrenade1Name;

    HandGrenade _primedGrenade2;
    public HandGrenade PrimedGrenade2 {
        get { return _primedGrenade2; }
    }
    private string _primedGrenade2Name;

    public Node MainNode;

    public HandGrenadeManager()
    {
    }

    public void PrimeGren(Player p, Ammunition GrenadeType, int grenNum)
    {
        // play grentimer
        AudioStreamPlayer gt = (AudioStreamPlayer)p.GetNode("GrenTimer");
        gt.Play();
        
        string projectileResource = "";
        float damage = 0f;
        switch (GrenadeType)
        {
            case Ammunition.FragGrenade:
                projectileResource = FragGrenade.ProjectileResource;
                damage = FragGrenade.Damage;
            break;
            case Ammunition.MFTGrenade:
            case Ammunition.ConcussionGrenade:
                projectileResource = ConcussionGrenade.ProjectileResource;
            break;
            case Ammunition.MIRVGrenade:
                projectileResource = MIRVGrenade.ProjectileResource;
                damage = MIRVGrenade.Damage;
            break;
        }
        PackedScene _projectileScene = (PackedScene)ResourceLoader.Load(projectileResource);
        HandGrenade _spawnedGrenade = (HandGrenade)_projectileScene.Instance();
        
        // add to scene so physics process is in use
        MainNode.AddChild(_spawnedGrenade);

        if (grenNum == 1)
        {
            _primedGrenade1Name = _spawnedGrenade.GetName();
            _primedGrenade1 = _spawnedGrenade;
        }
        else
        {
            _primedGrenade2Name = _spawnedGrenade.GetName();
            _primedGrenade2 = _spawnedGrenade;
        }

        _spawnedGrenade.Prime(p, damage, GrenadeType);
    }

    public void ThrowGren(Transform t, Player p, Ammunition GrenadeType, int grenNum)
    {
        if (MainNode.HasNode(_primedGrenade1Name))
        {
            if (grenNum == 1)
            {
                _primedGrenade1.Throw(t);
            }
            else
            {
                _primedGrenade2.Throw(t);
            }
        }
        else
        {
            PrimeGren(p, GrenadeType, grenNum);
        }

        if (grenNum == 1)
        {
            _primedGrenade1 = null;
        }
        else
        {
            _primedGrenade2 = null;   
        }
    }
}