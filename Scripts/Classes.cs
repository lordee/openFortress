using Godot;
using System;

abstract public class TFClass
{
    
    protected int _health;
    protected int _armour;
    protected Weapon _weapon1;
    protected Weapon _weapon2;
    protected Weapon _weapon3;
    protected Weapon _weapon4;
    protected Weapon _gren1;
    protected Weapon _gren2;
    protected int _maxShells;
    protected int _maxNails;
    protected int _maxRockets;
    protected int _maxCells;
    protected int _maxGren1;
    protected int _maxGren2;

    public int Health {
        get {
            return _health;           
        }
    }
    public int Armour {
        get {
            return _armour;           
        }
    }
    public Weapon Weapon1 {
        get {
            return _weapon1;           
        }
    }
    public Weapon Weapon2 {
        get {
            return _weapon2;           
        }
    }
    public Weapon Weapon3 {
        get {
            return _weapon3;           
        }
    }
    public Weapon Weapon4 {
        get {
            return _weapon4;           
        }
    }
    public Weapon Gren1 {
        get {
            return _gren1;           
        }   
    }
    public Weapon Gren2 {
        get {
            return _gren2;           
        }   
    }
    public int MaxShells {
        get {
            return _maxShells;           
        }   
    }
    public int MaxNails {
        get {
            return _maxNails;           
        }   
    }
    public int MaxRockets {
        get {
            return _maxRockets;           
        }   
    }
    public int MaxCells {
        get {
            return _maxCells;           
        }   
    }
    public int MaxGren1 {
        get {
            return _maxGren1;           
        }   
    }
    public int MaxGren2 {
        get {
            return _maxGren2;           
        }   
    }

    public void SpawnWeapons(Node camera)
    {
        if (Weapon1 != null)
        {
            Weapon1.Spawn(camera, "Weapon1");
        }

        if (Weapon2 != null)
        {
            Weapon2.Spawn(camera, "Weapon2");
        }

        if (Weapon3 != null)
        {
            //Weapon3.Spawn(camera, "Weapon3");
        }

        if (Weapon4 != null)
        {
            //Weapon4.Spawn(camera, "Weapon4");
        }
    }
}

public class Observer : TFClass
{
    public Observer()
    {
        _weapon1 = null;
        _weapon2 = null;
        _weapon3 = null;
        _weapon4 = null;
        _gren1 = null;
        _gren2 = null;
        _health = 0;
        _armour = 0;
        _maxShells = 0;
        _maxNails = 0;
        _maxRockets = 0;
        _maxCells = 0;
        _maxGren1 = 0;
        _maxGren2 = 0;
    }
}

public class Scout : TFClass
{
    public Scout()
    {
        _weapon1 = new NailGun();
        _weapon2 = new Shotgun();
        _weapon3 = new Axe();
        _weapon4 = null;
        _gren1 = new MFTGrenade();
        _gren2 = new ConcussionGrenade();
        _health = 75;
        _armour = 50;
        _maxShells = 50;
        _maxNails = 200;
        _maxRockets = 50;
        _maxCells = 50;
        _maxGren1 = 4;
        _maxGren2 = 4;
    }
}

public class Sniper : TFClass
{
    public Sniper()
    {
        _weapon1 = new SniperRifle();
        _weapon2 = new AutoRifle();
        _weapon3 = new NailGun();
        _weapon4 = new Axe();
        _gren1 = new FragGrenade();
        _gren2 = new Flare();
        _health = 85;
        _armour = 50;
        _maxShells = 75;
        _maxNails = 50;
        _maxRockets = 50;
        _maxCells = 50;
        _maxGren1 = 4;
        _maxGren2 = 4;
    }
}

public class Soldier : TFClass
{
    public Soldier()
    {
        _weapon1 = new RocketLauncher();
        _weapon2 = new SuperShotgun();
        _weapon3 = new Shotgun();
        _weapon4 = new Axe();
        _gren1 = new FragGrenade();
        _gren2 = new NailGrenade();
        _health = 100;
        _armour = 200;
        _maxShells = 100;
        _maxNails = 50;
        _maxRockets = 50;
        _maxCells = 50;
        _maxGren1 = 4;
        _maxGren2 = 2;
    }
}

public class Demoman : TFClass
{
    public Demoman()
    {
        _weapon1 = new GrenadeLauncher();
        _weapon2 = new PipebombLauncher();
        _weapon3 = new Shotgun();
        _weapon4 = new Axe();
        _gren1 = new FragGrenade();
        _gren2 = new MIRVGrenade();
        _health = 90;
        _armour = 120;
        _maxShells = 50;
        _maxNails = 50;
        _maxRockets = 50;
        _maxCells = 50;
        _maxGren1 = 4;
        _maxGren2 = 4;
    }
}

public class Medic : TFClass
{
    public Medic()
    {
        _weapon1 = new SuperNailGun();
        _weapon2 = new SuperShotgun();
        _weapon3 = new Shotgun();
        _weapon4 = new Syringe();
        _gren1 = new FragGrenade();
        _gren2 = new ConcussionGrenade();
        _health = 100;
        _armour = 80;
        _maxShells = 50;
        _maxNails = 150;
        _maxRockets = 50;
        _maxCells = 50;
        _maxGren1 = 4;
        _maxGren2 = 3;
    }
}

public class HWGuy : TFClass
{
    public HWGuy()
    {
        _weapon1 = new MiniGun();
        _weapon2 = new SuperShotgun();
        _weapon3 = new Shotgun();
        _weapon4 = new Axe();
        _gren1 = new FragGrenade();
        _gren2 = new MIRVGrenade();
        _health = 100;
        _armour = 300;
        _maxShells = 200;
        _maxNails = 50;
        _maxRockets = 50;
        _maxCells = 50;
        _maxGren1 = 4;
        _maxGren2 = 2;
    }
}

public class Pyro : TFClass
{
    public Pyro()
    {
        _weapon1 = new FlameThrower();
        _weapon2 = new PyroLauncher();
        _weapon3 = new Shotgun();
        _weapon4 = new Axe();
        _gren1 = new FragGrenade();
        _gren2 = new NapalmGrenade();
        _health = 100;
        _armour = 150;
        _maxShells = 50;
        _maxNails = 50;
        _maxRockets = 60;
        _maxCells = 200;
        _maxGren1 = 4;
        _maxGren2 = 4;
    }
}

public class Spy : TFClass
{
    public Spy()
    {
        _weapon1 = new Tranquiliser();
        _weapon2 = new SuperShotgun();
        _weapon3 = new NailGun();
        _weapon4 = new Knife();
        _gren1 = new FragGrenade();
        _gren2 = new GasGrenade();
        _health = 90;
        _armour = 50;
        _maxShells = 50;
        _maxNails = 100;
        _maxRockets = 50;
        _maxCells = 50;
        _maxGren1 = 4;
        _maxGren2 = 4;
    }
}

public class Engineer : TFClass
{
    public Engineer()
    {
        _weapon1 = new RailGun();
        _weapon2 = new SuperShotgun();
        _weapon3 = new Spanner();
        _weapon4 = null;
        _gren1 = new FragGrenade();
        _gren2 = new EMPGrenade();
        _health = 80;
        _armour = 50;
        _maxShells = 50;
        _maxNails = 50;
        _maxRockets = 50;
        _maxCells = 200;
        _maxGren1 = 4;
        _maxGren2 = 4;
    }
}