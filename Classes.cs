using Godot;
using System;

abstract public class TFClass
{
    abstract public int Health {
        get;
    }
    abstract public int Armour {
        get;
    }
    abstract public Weapon Weapon1 {
        get;
    }
    abstract public Weapon Weapon2 {
        get;
    }
    abstract public Weapon Weapon3 {
        get;
    }
    abstract public Weapon Weapon4 {
        get;
    }
    abstract public Weapon Gren1 {
        get;   
    }
    abstract public Weapon Gren2 {
        get;   
    }
    abstract public int MaxShells {
        get;   
    }
    abstract public int MaxNails {
        get;   
    }
    abstract public int MaxRockets {
        get;   
    }
    abstract public int MaxCells {
        get;   
    }
    abstract public int MaxGren1 {
        get;   
    }
    abstract public int MaxGren2 {
        get;   
    }
}

public class Observer : TFClass
{
    override public int Health {
        get {
            return 0;
        }
    }
    override public int Armour {
        get {
            return 0;
        }   
    }
    override public Weapon Weapon1 {
        get {
            return null;
        }   
    }
    override public Weapon Weapon2 {
        get {
            return null;
        }   
    }
    override public Weapon Weapon3 {
        get {
            return null;
        }   
    }
    override public Weapon Weapon4 {
        get {
            return null;
        }   
    }

    override public Weapon Gren1 {
        get {
            return null;
        }   
    }
    override public Weapon Gren2 {
        get {
            return null;
        }   
    }

    override public int MaxShells {
        get {
            return 0;
        }   
    }
    override public int MaxNails {
        get {
            return 0;
        }   
    }
    override public int MaxRockets {
        get {
            return 0;
        }   
    }
    override public int MaxCells {
        get {
            return 0;
        }   
    }
    override public int MaxGren1 {
        get {
            return 0;
        }   
    }
    override public int MaxGren2 {
        get {
            return 0;
        }   
    }
}

public class Scout : TFClass
{
    override public int Health {
        get {
            return 75;
        }
    }
    
    override public int Armour {
        get {
            return 50;
        }   
    }
    override public Weapon Weapon1 {
        get {
            return new NailGun();
        }   
    }
    override public Weapon Weapon2 {
        get {
            return new Shotgun();
        }   
    }
    override public Weapon Weapon3 {
        get {
            return new Axe();
        }   
    }
    override public Weapon Weapon4 {
        get {
            return null;
        }   
    }
    override public Weapon Gren1 {
        get {
            return new MFTGrenade();
        }   
    }
    override public Weapon Gren2 {
        get {
            return new ConcussionGrenade();
        }   
    }

    override public int MaxShells {
        get {
            return 50;
        }
    }
    override public int MaxNails {
        get {
            return 200;
        }
    }
    override public int MaxRockets {
        get {
            return 50;
        }
    }
    override public int MaxCells {
        get {
            return 50;
        }
    }
    override public int MaxGren1 {
        get {
            return 4;
        }
    }
    override public int MaxGren2 {
        get {
            return 4;
        }
    }
}

public class Sniper : TFClass
{
    override public int Health {
        get {
            return 85;
        }
    }
    
    override public int Armour {
        get {
            return 50;
        }   
    }
    override public Weapon Weapon1 {
        get {
            return new SniperRifle();
        }   
    }
    override public Weapon Weapon2 {
        get {
            return new AutoRifle();
        }   
    }
    override public Weapon Weapon3 {
        get {
            return new NailGun();
        }   
    }
    override public Weapon Weapon4 {
        get {
            return new Axe();
        }   
    }
    override public Weapon Gren1 {
        get {
            return new FragGrenade();
        }   
    }
    override public Weapon Gren2 {
        get {
            return new Flare();
        }   
    }

    override public int MaxShells {
        get {
            return 75;
        }
    }
    override public int MaxNails {
        get {
            return 50;
        }
    }
    override public int MaxRockets {
        get {
            return 50;
        }
    }
    override public int MaxCells {
        get {
            return 50;
        }
    }
    override public int MaxGren1 {
        get {
            return 4;
        }
    }
    override public int MaxGren2 {
        get {
            return 4;
        }
    }
}
public class Soldier : TFClass
{
    override public int Health {
        get {
            return 100;
        }
    }
    
    override public int Armour {
        get {
            return 200;
        }   
    }
    override public Weapon Weapon1 {
        get {
            return new RocketLauncher();
        }   
    }
    override public Weapon Weapon2 {
        get {
            return new SuperShotgun();
        }   
    }
    override public Weapon Weapon3 {
        get {
            return new Shotgun();
        }   
    }
    override public Weapon Weapon4 {
        get {
            return new Axe();
        }   
    }
    override public Weapon Gren1 {
        get {
            return new FragGrenade();
        }   
    }
    override public Weapon Gren2 {
        get {
            return new NailGrenade();
        }   
    }

    override public int MaxShells {
        get {
            return 100;
        }
    }
    override public int MaxNails {
        get {
            return 50;
        }
    }
    override public int MaxRockets {
        get {
            return 50;
        }
    }
    override public int MaxCells {
        get {
            return 50;
        }
    }
    override public int MaxGren1 {
        get {
            return 4;
        }
    }
    override public int MaxGren2 {
        get {
            return 2;
        }
    }
}
public class Demoman : TFClass
{
    override public int Health {
        get {
            return 90;
        }
    }
    
    override public int Armour {
        get {
            return 120;
        }   
    }
    override public Weapon Weapon1 {
        get {
            return new GrenadeLauncher();
        }   
    }
    override public Weapon Weapon2 {
        get {
            return new PipebombLauncher();
        }   
    }
    override public Weapon Weapon3 {
        get {
            return new Shotgun();
        }   
    }
    override public Weapon Weapon4 {
        get {
            return new Axe();
        }   
    }
    override public Weapon Gren1 {
        get {
            return new FragGrenade();
        }   
    }
    override public Weapon Gren2 {
        get {
            return new MIRVGrenade();
        }   
    }

    override public int MaxShells {
        get {
            return 50;
        }
    }
    override public int MaxNails {
        get {
            return 50;
        }
    }
    override public int MaxRockets {
        get {
            return 50;
        }
    }
    override public int MaxCells {
        get {
            return 50;
        }
    }
    override public int MaxGren1 {
        get {
            return 4;
        }
    }
    override public int MaxGren2 {
        get {
            return 4;
        }
    }
}
public class Medic : TFClass
{
    override public int Health {
        get {
            return 100;
        }
    }
    
    override public int Armour {
        get {
            return 80;
        }   
    }
    override public Weapon Weapon1 {
        get {
            return new SuperNailGun();
        }   
    }
    override public Weapon Weapon2 {
        get {
            return new SuperShotgun();
        }   
    }
    override public Weapon Weapon3 {
        get {
            return new Shotgun();
        }   
    }
    override public Weapon Weapon4 {
        get {
            return new Syringe();
        }   
    }
    override public Weapon Gren1 {
        get {
            return new FragGrenade();
        }   
    }
    override public Weapon Gren2 {
        get {
            return new ConcussionGrenade();
        }   
    }

    override public int MaxShells {
        get {
            return 50;
        }
    }
    override public int MaxNails {
        get {
            return 150;
        }
    }
    override public int MaxRockets {
        get {
            return 50;
        }
    }
    override public int MaxCells {
        get {
            return 50;
        }
    }
    override public int MaxGren1 {
        get {
            return 4;
        }
    }
    override public int MaxGren2 {
        get {
            return 3;
        }
    }
}
public class HWGuy : TFClass
{
    override public int Health {
        get {
            return 100;
        }
    }
    
    override public int Armour {
        get {
            return 300;
        }   
    }
    override public Weapon Weapon1 {
        get {
            return new MiniGun();
        }   
    }
    override public Weapon Weapon2 {
        get {
            return new SuperShotgun();
        }   
    }
    override public Weapon Weapon3 {
        get {
            return new Shotgun();
        }   
    }
    override public Weapon Weapon4 {
        get {
            return new Axe();
        }   
    }
    override public Weapon Gren1 {
        get {
            return new FragGrenade();
        }   
    }
    override public Weapon Gren2 {
        get {
            return new MIRVGrenade();
        }   
    }

    override public int MaxShells {
        get {
            return 200;
        }
    }
    override public int MaxNails {
        get {
            return 50;
        }
    }
    override public int MaxRockets {
        get {
            return 50;
        }
    }
    override public int MaxCells {
        get {
            return 50;
        }
    }
    override public int MaxGren1 {
        get {
            return 4;
        }
    }
    override public int MaxGren2 {
        get {
            return 2;
        }
    }
}
public class Pyro : TFClass
{
    override public int Health {
        get {
            return 100;
        }
    }
    
    override public int Armour {
        get {
            return 150;
        }   
    }
    override public Weapon Weapon1 {
        get {
            return new FlameThrower();
        }   
    }
    override public Weapon Weapon2 {
        get {
            return new PyroLauncher();
        }   
    }
    override public Weapon Weapon3 {
        get {
            return new Shotgun();
        }   
    }
    override public Weapon Weapon4 {
        get {
            return new Axe();
        }   
    }
    override public Weapon Gren1 {
        get {
            return new FragGrenade();
        }   
    }
    override public Weapon Gren2 {
        get {
            return new NapalmGrenade();
        }   
    }

    override public int MaxShells {
        get {
            return 50;
        }
    }
    override public int MaxNails {
        get {
            return 50;
        }
    }
    override public int MaxRockets {
        get {
            return 60;
        }
    }
    override public int MaxCells {
        get {
            return 200;
        }
    }
    override public int MaxGren1 {
        get {
            return 4;
        }
    }
    override public int MaxGren2 {
        get {
            return 4;
        }
    }
}
public class Spy : TFClass
{
    override public int Health {
        get {
            return 90;
        }
    }
    
    override public int Armour {
        get {
            return 50;
        }   
    }
    override public Weapon Weapon1 {
        get {
            return new Tranquiliser();
        }   
    }
    override public Weapon Weapon2 {
        get {
            return new SuperShotgun();
        }   
    }
    override public Weapon Weapon3 {
        get {
            return new NailGun();
        }   
    }
    override public Weapon Weapon4 {
        get {
            return new Knife();
        }   
    }
    override public Weapon Gren1 {
        get {
            return new FragGrenade();
        }   
    }
    override public Weapon Gren2 {
        get {
            return new GasGrenade();
        }   
    }

    override public int MaxShells {
        get {
            return 50;
        }
    }
    override public int MaxNails {
        get {
            return 200;
        }
    }
    override public int MaxRockets {
        get {
            return 50;
        }
    }
    override public int MaxCells {
        get {
            return 50;
        }
    }
    override public int MaxGren1 {
        get {
            return 4;
        }
    }
    override public int MaxGren2 {
        get {
            return 4;
        }
    }
}
public class Engineer : TFClass
{
    override public int Health {
        get {
            return 80;
        }
    }
    
    override public int Armour {
        get {
            return 50;
        }   
    }
    override public Weapon Weapon1 {
        get {
            return new RailGun();
        }   
    }
    override public Weapon Weapon2 {
        get {
            return new SuperShotgun();
        }   
    }
    override public Weapon Weapon3 {
        get {
            return new Spanner();
        }   
    }
    override public Weapon Weapon4 {
        get {
            return null;
        }   
    }
    override public Weapon Gren1 {
        get {
            return new FragGrenade();
        }   
    }
    override public Weapon Gren2 {
        get {
            return new EMPGrenade();
        }   
    }

    override public int MaxShells {
        get {
            return 50;
        }
    }
    override public int MaxNails {
        get {
            return 50;
        }
    }
    override public int MaxRockets {
        get {
            return 50;
        }
    }
    override public int MaxCells {
        get {
            return 200;
        }
    }
    override public int MaxGren1 {
        get {
            return 4;
        }
    }
    override public int MaxGren2 {
        get {
            return 4;
        }
    }
}
