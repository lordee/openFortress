using Godot;
using System;

// these will no doubt move to their own classes and godot scenes/nodes
// just use these as definitions for now

public class Weapon
{
    public int Damage = 0;
}

public class FragGrenade : Weapon
{
    public FragGrenade() {
        this.Damage = 100;
    }
}

// magic fly tool grenade - old concussion
public class MFTGrenade : Weapon
{
    public MFTGrenade() {
        this.Damage = 0;
    }
}

public class ConcussionGrenade : Weapon
{
    public ConcussionGrenade() {
        this.Damage = 0;
    }
}

// this is useless, replace it with something
public class Flare : Weapon
{

}

public class NailGrenade : Weapon
{
    public NailGrenade() {
        this.Damage = 0;
    }
}

public class MIRVGrenade : Weapon
{
    public MIRVGrenade() {
        this.Damage = 100;
    }
}

public class NapalmGrenade : Weapon
{
    public NapalmGrenade() {
        this.Damage = 20;
    }
}

public class GasGrenade : Weapon
{
    public GasGrenade() {
        this.Damage = 10;
    }
}

public class EMPGrenade : Weapon
{
    public EMPGrenade() {
        this.Damage = 0;
    }
}

public class Axe : Weapon
{
    public Axe() {
        this.Damage = 10;
    }
}

public class Shotgun : Weapon
{
    public Shotgun () {
        this.Damage = 10;
    }
}

public class SuperShotgun : Weapon
{
    public SuperShotgun() {
        this.Damage = 20;
    }
}

public class NailGun : Weapon
{
    public NailGun() {
        this.Damage = 10;
    }
}

public class SniperRifle : Weapon
{
    public SniperRifle() {
        this.Damage = 10;
    }
}

public class AutoRifle : Weapon
{
    public AutoRifle() {
        this.Damage = 10;
    }
}

public class SuperNailGun : Weapon
{
    public SuperNailGun() {
        this.Damage = 20;
    }
}

public class GrenadeLauncher : Weapon
{
    public GrenadeLauncher() {
        this.Damage = 80;
    }
}

public class PipebombLauncher : Weapon
{
    public PipebombLauncher() {
        this.Damage = 80;
    }
}

public class RocketLauncher : Weapon
{
    public RocketLauncher() {
        this.Damage = 80;
    }
}



public class Syringe : Weapon
{
    public Syringe() {
        this.Damage = 10;
    }
}

public class MiniGun : Weapon
{
    public MiniGun() {
        this.Damage = 10;
    }
}

public class FlameThrower : Weapon
{
    public FlameThrower() {
        this.Damage = 10;
    }
}

public class PyroLauncher : Weapon
{
    public PyroLauncher() {
        this.Damage = 10;
    }
}

public class Tranquiliser : Weapon
{
    public Tranquiliser() {
        this.Damage = 10;
    }
}

public class Knife : Weapon
{
    public Knife() {
        this.Damage = 100;
    }
}

public class RailGun : Weapon
{
    public RailGun() {
        this.Damage = 20;
    }
}

public class Spanner : Weapon
{
    public Spanner() {
        this.Damage = 10;
    }
}

