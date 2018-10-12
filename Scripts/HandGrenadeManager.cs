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
    public Node MainNode;

    public HandGrenadeManager()
    {
    }

    public void PrimeGren1(Player p, Ammunition GrenadeType)
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
        }
        PackedScene _projectileScene = (PackedScene)ResourceLoader.Load(projectileResource);
        HandGrenade _spawnedGrenade = (HandGrenade)_projectileScene.Instance();
        
        // add to scene so physics process is in use
        MainNode.AddChild(_spawnedGrenade);

        _primedGrenade1Name = _spawnedGrenade.GetName();

        _spawnedGrenade.Prime(p, damage);
        _primedGrenade1 = _spawnedGrenade;
    }

    public void PrimeGren2()
    {

    }

    public void ThrowGren1(Transform t, Player p, Ammunition GrenadeType)
    {
        if (MainNode.HasNode(_primedGrenade1Name))
        {
            _primedGrenade1.Throw(t);
        }
        else
        {
            PrimeGren1(p, GrenadeType);
        }
        _primedGrenade1 = null;
    }
}