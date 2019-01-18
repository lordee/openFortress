using Godot;
using System;
using System.Collections.Generic;

class DiseasedData
{
    public Player Attacker;
    public float TimeSinceDiseased;
    public Weapon Inflictor;

    public DiseasedData(Player attacker, string inflictor, float timeSinceDiseased)
    {
        Attacker = attacker;
        TimeSinceDiseased = timeSinceDiseased;
        switch (inflictor)
        {
            case "syringe":
                Inflictor = new Syringe();
            break;
        }
    }
}
public class Player : KinematicBody
{
    float mouseSensitivity = 0.2f;
    float cameraAngle = 0F;

    private int _pipebombLimit = 7;

    // states
    private float _tranquilisedLength = 0f;
    private bool _tranquilised = false;
    private float _timeSinceTranquilised = 0f;
    public float TimeSinceTranquilised {
        get { return _timeSinceTranquilised; }
        set {
            _timeSinceTranquilised = value;
            if (_tranquilised)
            {
                if (_timeSinceTranquilised >= _tranquilisedLength)
                {
                    Tranquilised = false;
                }
            }
        }
    }
    public bool Tranquilised {
        get { return _tranquilised; }
        set {
            _tranquilised = value;
            _timeSinceTranquilised = 0f;
        }
    }

    private List<DiseasedData> _diseasedBy = new List<DiseasedData>();
    private float _diseasedInterval = 0f;

    private bool _concussed = false;
    public bool Concussed {
        get { return _concussed; }
        set {
            if (!_concussed)
            {
                // only set new position if they aren't currently concussed
                _concussionCrosshairDestination = GetConcussionCrosshairDestination(value);
            }
            _concussed = value;
            _timeSinceConcussed = 0f;
            if (!value)
            {
                // reset crosshair coords
                Crosshair.Position = GetConcussionCrosshairDestination(value);
            }
        }
    }
    private float _concussedLength = 0f;
    private float _timeSinceConcussed = 0f;
    public float TimeSinceConcussed {
        get { return _timeSinceConcussed; }
        set {
            _timeSinceConcussed = value;
            if (_concussed)
            {
                if (_timeSinceConcussed >= _concussedLength)
                {
                    Concussed = false;
                }
            }
        }
    }
    private Vector2 _concussionCrosshairDestination;

    private HandGrenade _primedGrenade;
    public HandGrenade PrimedGrenade
    {
        get { return _primedGrenade; }
        set { _primedGrenade = value; }
    }

    // physics
    private float gravity = 27.0f;
    private float friction = 6;
    private Vector3 up = new Vector3(0,1,0);
    // stairs
    float maxStairAngle = 20F;
    float stairJumpHeight = 9F;

    // movement
    public float moveSpeed = 15.0f;               // Ground move speed
    public float runAcceleration = 14.0f;         // Ground accel
    public float runDeacceleration = 10.0f;       // Deacceleration that occurs when running on the ground
    public float airAcceleration = 2.0f;          // Air accel
    public float airDecceleration = 2.0f;         // Deacceleration experienced when opposite strafing
    public float airControl = 0.3f;               // How precise air control is
    public float sideStrafeAcceleration = 50.0f;  // How fast acceleration occurs to get up to sideStrafeSpeed
    public float sideStrafeSpeed = 1.0f;          // What the max speed to generate when side strafing
    public float jumpSpeed = 8.0f;                // The speed at which the character's up axis gains when hitting jump
    public float moveScale = 1.0f;
   
    private Vector3 moveDirectionNorm = new Vector3();
    private Vector3 playerVelocity = new Vector3();
    
   
    // Nodes
    Spatial _head;
    Camera camera;
    RayCast stairCatcher;
    Label HealthLabel;
    Label ArmourLabel;
    private Sprite _crosshair;
    public Sprite Crosshair {
        get {
            if (_crosshair == null)
            {
                _crosshair = (Sprite)GetNode("/root/OpenFortress/Main/UI/Crosshair");
            }
            return _crosshair;
        }
    }

    PlayerController _playerController = null;
    public PlayerController PlayerController {
        get { return _playerController; }
    }

    private Main _mainNode;
    public Main MainNode {
        get {
            if (_mainNode == null)
            {
                _mainNode = (Main)GetNode("/root/OpenFortress/Main");
            }
            return _mainNode;
        }
    }

    private Network _network;
    public Network Network {
        get {
            if (_network == null)
            {
                _network = (Network)GetNode("/root/OpenFortress/Network");
            }
            return _network;
        }
    }

    // state
    private int _teamID = 9;
    public int TeamID { 
        get {
            return _teamID;
        }
        set {
            Class = new Observer();
            _teamID = value;

            this.Spawn(MainNode.GetNextSpawn(value));
        }
    }
    private TFClass _class = new Observer();
    public TFClass Class {
        get {
            return _class;
        }
        set {
            _class = value;
            if (value.GetType() != typeof(Observer))
            {   
                _class.SpawnWeapons(MainNode, this.camera);
            }
            
            // respawn instantly on class change
            this.Spawn(MainNode.GetNextSpawn(this.TeamID));
        }
    }
    private int _currentArmour;
    public int CurrentArmour 
    {
        get {
            return _currentArmour;
        }
        set {
            _currentArmour = value;
            ArmourLabel.Text = value.ToString();
        }
    }

    private int _currentHealth;
    public int CurrentHealth 
    {
        get {
            return _currentHealth;
        }
        set {
            _currentHealth = value;
            HealthLabel.Text = value.ToString();
        }
    }

    private int _currentShells = 0;
    private int _currentNails = 0;
    private int _currentRockets = 0;
    private int _currentCells = 0;
    private int _currentGren1 = 0;
    private int _currentGren2 = 0;

    public int CurrentShells {
        get { return _currentShells; }
        set {
            _currentShells = value;
            // update gui too
        }
    }

    public int CurrentRockets {
        get { return _currentRockets; }
        set {
            _currentRockets = value;
            // update gui too
        }
    }
    public int CurrentCells {
        get { return _currentCells; }
        set {
            _currentCells = value;
            // update gui too
        }
    }
    
    private int _activeAmmo;
    private int ActiveAmmo
    {
        get {
            return _activeAmmo;
        }
        set {
            switch (_activeWeapon.AmmoType)
            {
                case Ammunition.Shells:
                        this._currentShells = value;
                    break;
                    case Ammunition.Nails:
                        this._currentNails = value;
                    break;
                    case Ammunition.Rockets:
                        this._currentRockets = value;
                    break;
                    case Ammunition.Cells:
                        this._currentCells = value;
                    break;
            }
            _activeAmmo = value;
        }
    }
    private Weapon _activeWeapon;
    public Weapon ActiveWeapon
    {
        get {
            return _activeWeapon;
        }
        set {
            if (value != null)
            {
                // if player has min amount of ammunition required
                int ammoReq = value.MinAmmoRequired;
                int ammoQty = 0;
                switch (value.AmmoType)
                {
                    case Ammunition.Shells:
                        ammoQty = this._currentShells;
                    break;
                    case Ammunition.Nails:
                        ammoQty = this._currentNails;
                    break;
                    case Ammunition.Rockets:
                        ammoQty = this._currentRockets;
                    break;
                    case Ammunition.Cells:
                        ammoQty = this._currentCells;
                    break;
                }

                if (ammoQty >= ammoReq)
                {
                    // hide current activeweapon
                    if (_activeWeapon != null)
                    {
                        _activeWeapon.WeaponMesh.Visible = false;
                    }

                    // set active weapon
                    _activeWeapon = value;
                    ActiveAmmo = ammoQty;

                    // make active weapon visible
                    _activeWeapon.WeaponMesh.Visible = true;
                    _activeWeapon.AmmoLeft = ActiveAmmo;
                }
            }
        }
    }
    private List<Pipebomb> _activePipebombs = new List<Pipebomb>();
    public List<Pipebomb> ActivePipebombs {
        get { return _activePipebombs; }
    }
        
    bool climbLadder = false;
    bool firstEnter = true; // bug? on first load, body entered is called

    // shooting
    Vector2 cameraCenter;

    // bunnyhopping
    private bool wishJump = false;
    private bool touchingGround = false;

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
        _head = (Spatial)GetNode("Head");
        camera = (Camera)_head.GetNode("Camera");
        stairCatcher = (RayCast)GetNode("StairCatcher");
        HealthLabel = (Label)GetNode("/root/OpenFortress/Main/UI/HealthLabel");
        ArmourLabel = (Label)GetNode("/root/OpenFortress/Main/UI/ArmourLabel");
        cameraCenter.x = OS.GetWindowSize().x / 2;
        cameraCenter.y = OS.GetWindowSize().y / 2;
        _playerController = (PlayerController)GetNode("PlayerController");
        _playerController.Init(this);

        // enable ladders
        var ladders = GetTree().GetNodesInGroup("Ladders");
        foreach (Area l in ladders)
        {
            l.Connect("body_entered", this, "_on_Ladder_body_entered");
            l.Connect("body_exited", this, "_on_Ladder_body_exited");
        }
    }

    public void RotateHead(float x, float y)
    {
        _head.RotateY(Mathf.Deg2Rad(x * mouseSensitivity));

        // limit how far up/down we look
        // invert mouse
        float change = y * mouseSensitivity;
        if (cameraAngle + change < 90F && cameraAngle + change > -90F)
        {
            camera.RotateX(Mathf.Deg2Rad(change));
            cameraAngle += change;
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        TimeSinceTranquilised += delta;
        foreach (DiseasedData dd in _diseasedBy)
        {
            dd.TimeSinceDiseased += delta;
            if (dd.TimeSinceDiseased >= _diseasedInterval)
            {
                this.TakeDamage(this.Transform, dd.Inflictor.GetType().ToString().ToLower(), dd.Inflictor.InflictLength, dd.Attacker, dd.Inflictor.Damage);
            }
        }
        
        TimeSinceConcussed += delta;
        if (Concussed)
        {
            // if crosshair within percentage of final destination, set new coords
            if (Math.Abs(_concussionCrosshairDestination.x - Crosshair.Position.x) < 10 
                && Math.Abs(_concussionCrosshairDestination.y - Crosshair.Position.y) < 10)
                {
                    _concussionCrosshairDestination = GetConcussionCrosshairDestination(true);
                }
            
            // move crosshair towards coords (change this later to an arc that changes constantly)
            Vector2 targ = (Crosshair.Position - _concussionCrosshairDestination).Normalized();

            Vector2 motion = targ * 50 * delta;

            Crosshair.Position -= motion;
        }
        
        if (Input.GetMouseMode() == Input.MouseMode.Captured)
        {
            // process inputs -- should this be in normal process instead of physics process?
            foreach (Impulse imp in _playerController.Impulses)
            {
                switch (imp)
                {
                    case Impulse.Attack:
                        this.Shoot();
                    break;
                    case Impulse.Slot1:
                        ActiveWeapon = this.Class.Weapon1;
                    break;
                    case Impulse.Slot2:
                        ActiveWeapon = this.Class.Weapon2;
                    break;
                    case Impulse.Slot3:
                        ActiveWeapon = this.Class.Weapon3;
                    break;
                    case Impulse.Slot4:
                        ActiveWeapon = this.Class.Weapon4;
                    break;
                    case Impulse.Detpipe:
                        this.Detpipe();
                    break;
                    case Impulse.Gren1:
                        if (this.Class.HandGrenadeManager.PrimedGrenade1 != null)
                        {
                            // throw it
                            this.Class.HandGrenadeManager.ThrowGren(camera.GetGlobalTransform(), this, this.Class.Gren1, 1);
                        }
                        else if (this._currentGren1 > 0)
                        {
                            // prime new gren1
                            this.Class.HandGrenadeManager.PrimeGren(this, this.Class.Gren1, 1);
                        
                            _currentGren1 -= 1;
                        }
                    break;
                    case Impulse.Gren2:
                        if (this.Class.HandGrenadeManager.PrimedGrenade2 != null)
                        {
                            // throw it
                            this.Class.HandGrenadeManager.ThrowGren(camera.GetGlobalTransform(), this, this.Class.Gren2, 2);
                        }
                        else if (this._currentGren2 > 0)
                        {
                            // prime new gren2
                            this.Class.HandGrenadeManager.PrimeGren(this, this.Class.Gren2, 2);
                        
                            _currentGren2 -= 1;
                        }
                    break;
                }
            }
            _playerController.Impulses.Clear();

            _playerController.Transform = this.Transform;
            throw new NotImplementedException();
            // if controlling client, send your transform
            if ()
            {

            }
            // if other player, check for transform updates
            else 
            {
                if (_playerController.Transform != new Transform())
                {
                    // TODO should probably check how far they are from each other and rubber band instead of straight apply
                    this.Transform = _playerController.Transform;
                    _playerController.Transform = new Transform();
                }
            }
            
            if (ActiveWeapon != null)
            {
                ActiveWeapon.PhysicsProcess(delta);
            }

            QueueJump();
            if (touchingGround || climbLadder)
            {
                GroundMove(delta);
            }
            else
            {
                AirMove(delta);
            }
            
            playerVelocity = this.MoveAndSlide(playerVelocity, up);
            touchingGround = IsOnFloor();
            float speed = playerVelocity.Length();
            //GD.Print("Speed: " + speed.ToString());
        }
    }

    public void AddActivePipebomb(Pipebomb p)
    {
        if (_activePipebombs.Count >= _pipebombLimit)
        {
            // get oldest and explode
            _activePipebombs[0].Explode(null, p.WeaponOwner.Damage);
            _activePipebombs.Remove(_activePipebombs[0]);
        }
        _activePipebombs.Add(p);
    }

    private void Detpipe()
    {
        bool det = false;
        if (_activePipebombs.Count > 0)
        {
            Pipebomb test = _activePipebombs[0];
            if (test.WeaponOwner.TimeSinceLastShot >= .3f)
            {
                det = true;
            }
        }
        if (det)
        {
            foreach (Pipebomb p in _activePipebombs)
            {
                p.Explode(null, p.WeaponOwner.Damage);
            }
            _activePipebombs.Clear();
        }
    }

    public void Heal(Player attacker, Weapon inflictor)
    {
        switch (inflictor.GetType().ToString().ToLower())
        {
            case "syringe":
                _diseasedBy.Clear();
                if (this.CurrentHealth < 100)
                {
                    this.CurrentHealth = 100;
                }
                else
                {
                    this.CurrentHealth = 150;
                }
            break;
            case "spanner":
                if (this.CurrentArmour < this.Class.Armour)
                {
                    int maxToHeal = this.CurrentArmour + 40 > this.Class.Armour ? this.Class.Armour - this.CurrentArmour : 40;
                    int toHeal = attacker.CurrentCells >= maxToHeal / 4 ? maxToHeal : attacker.CurrentCells * 4;

                    this.CurrentArmour += toHeal;
                    attacker.CurrentCells -= toHeal / 4;
                }
            break;
        }
    }

    public void Inflict(string inflictorType, float inflictLength, Player attacker)
    {
        // special stuff
        switch (inflictorType.ToLower())
        {
            case "tranquiliser":
                this.Tranquilised = true;
                _tranquilisedLength = inflictLength;
            break;
            case "syringe":
                _diseasedBy.Add(new DiseasedData(attacker, inflictorType, 0f));
                _diseasedInterval = inflictLength;
            break;
            case "concussiongrenade":
                Concussed = true;
                _concussedLength = inflictLength;
            break;
        }
    }

    public void TakeDamage(Transform inflictorTransform, string inflictorType, float inflictLength, Player attacker, float damage)
    {
        Inflict(inflictorType, inflictLength, attacker);
        
        // take from armour and health
        int a = CurrentArmour;
        int h = CurrentHealth;
        // calc max a used (4 armour to every 1 health of damage)
        float aUsed = damage / 5 * 4;

        if (aUsed >= a)
        {
            aUsed = a;
        }
        CurrentArmour -= Convert.ToInt16(aUsed);

        float hUsed = damage - aUsed;

        if (h > hUsed)
        {
            // they survive
            CurrentHealth -= Convert.ToInt16(hUsed);
        }
        else
        {
            this.Die(inflictorType, attacker);
            return;
        }

        // add velocity
        AddVelocity(inflictorTransform.origin, damage);
    }

    public void AddVelocity(Vector3 inflictorOrigin, float speedToAdd)
    {
        GD.Print("p origin: " + this.Transform.origin);
        GD.Print("inflict origin: " + inflictorOrigin);
        GD.Print("damage to speed: " + speedToAdd);
        Vector3 dir = this.Transform.origin - inflictorOrigin;
        dir = dir.Normalized();
        dir = dir * speedToAdd;
        this.playerVelocity += dir;
    }

    private void Die(string inflictorType, Player attacker)
    {
        throw new NotImplementedException();
        // death sound
        // orientation change
        // respawn on input
        // log the death
    }

    private Vector2 GetConcussionCrosshairDestination(bool concussed)
    {
        Vector2 pos = new Vector2();
        if (concussed)
        {
            Random ran = new Random();
            pos.x = ran.Next(256,768);
            pos.y = ran.Next(150,450);
        }
        else
        {
            // center it
            pos.x = 512;
            pos.y = 300;
        }

        return pos;
    }

    private void Shoot()
    {
        // if there's enough ammunition
        GD.Print("ActiveAmmo: " + ActiveAmmo);
        if (ActiveAmmo >= ActiveWeapon.MinAmmoRequired)
        {
            // if weapon is off cooldown
            if (ActiveWeapon.Shoot(camera, Crosshair.Position, this))
            {
                // modify current ammunition
                ActiveAmmo -= ActiveWeapon.MinAmmoRequired;
            }
        }
        else
        {
            GD.Print("You do not have enough ammunition to fire this gun.");
        }       
    }

    public void Spawn(Vector3 loc)
    {
        this.SetTranslation(loc);
        // do other stuff around being dead etc 
        
        GD.Print(this.Class.ToString());
        this.CurrentHealth = this.Class.Health;
        this.CurrentArmour = this.Class.Armour / 2;
        this._currentShells = Math.Abs(this.Class.MaxShells / 2);
        this._currentNails = Math.Abs(this.Class.MaxNails / 2);
        this._currentRockets = Math.Abs(this.Class.MaxRockets / 2);
        this._currentCells = Math.Abs(this.Class.MaxCells / 2);
        this._currentGren1 = Math.Abs(this.Class.MaxGren1 / 2);
        this._currentGren2 = Math.Abs(this.Class.MaxGren2 / 2);
        if (Class.Weapon1 != null)
        {
            ActiveWeapon = Class.Weapon1;
        }
    }

    private void QueueJump()
    {
        if (_playerController.move_up == 1 && !wishJump)
        {
            wishJump = true;
        }
        if (_playerController.move_up == -1)
        {
            wishJump = false;
        }
    }
    private void AirMove(float delta)
    {
        Vector3 wishdir = new Vector3();
        Basis aim = camera.GetGlobalTransform().basis;
        float wishvel = airAcceleration;
        float accel;
        
        float scale = CmdScale();

        wishdir += aim.x * _playerController.move_right;
        wishdir -= aim.z * _playerController.move_forward;

        float wishspeed = wishdir.Length();
        wishspeed *= _tranquilised ? moveSpeed / 2 : moveSpeed;

        wishdir = wishdir.Normalized();
        moveDirectionNorm = wishdir;

        // CPM: Aircontrol
        float wishspeed2 = wishspeed;
        if (playerVelocity.Dot(wishdir) < 0)
            accel = airDecceleration;
        else
            accel = airAcceleration;
        // If the player is ONLY strafing left or right
        if(_playerController.move_forward == 0 && _playerController.move_right != 0)
        {
            if(wishspeed > sideStrafeSpeed)
            {
                wishspeed = sideStrafeSpeed;
            }
                
            accel = sideStrafeAcceleration;
        }

        Accelerate(wishdir, wishspeed, accel, delta);
        if(airControl > 0)
        {
            AirControl(wishdir, wishspeed2, delta);
        }
        // !CPM: Aircontrol

        // Apply gravity
        if (!climbLadder)
        {
            playerVelocity.y -= gravity * delta;
        }
    }

    /**
     * Air control occurs when the player is in the air, it allows
     * players to move side to side much faster rather than being
     * 'sluggish' when it comes to cornering.
     */
    private void AirControl(Vector3 wishdir, float wishspeed, float delta)
    {
        float zspeed;
        float speed;
        float dot;
        float k;

        // Can't control movement if not moving forward or backward
        if(Mathf.Abs(_playerController.move_forward) < 0.001 || Mathf.Abs(wishspeed) < 0.001)
            return;
        zspeed = playerVelocity.y;
        playerVelocity.y = 0;
        // Next two lines are equivalent to idTech's VectorNormalize()
        speed = playerVelocity.Length();
        playerVelocity = playerVelocity.Normalized();

        dot = playerVelocity.Dot(wishdir);
        k = 32;
        k *= airControl * dot * dot * delta;

        // Change direction while slowing down
        if (dot > 0)
        {
            playerVelocity.x = playerVelocity.x * speed + wishdir.x * k;
            playerVelocity.y = playerVelocity.y * speed + wishdir.y * k;
            playerVelocity.z = playerVelocity.z * speed + wishdir.z * k;

            playerVelocity = playerVelocity.Normalized();
            moveDirectionNorm = playerVelocity;
        }

        playerVelocity.x *= speed;
        playerVelocity.y = zspeed; // Note this line
        playerVelocity.z *= speed;
    }

    private void GroundMove(float delta)
    {
        Vector3 wishDir = new Vector3();
        Basis aim = camera.GetGlobalTransform().basis;

        // Do not apply friction if the player is queueing up the next jump
        if (!wishJump)
        {
            ApplyFriction(1.0f, delta);
        }
        else
        {
            ApplyFriction(0, delta);
        }

        float scale = CmdScale();

        wishDir += aim.x * _playerController.move_right;
        wishDir -= aim.z * _playerController.move_forward;
        wishDir = wishDir.Normalized();
        moveDirectionNorm = wishDir;

        float wishSpeed = wishDir.Length();
        wishSpeed *= _tranquilised ? moveSpeed / 2 : moveSpeed;
        Accelerate(wishDir, wishSpeed, runAcceleration, delta);
       
        if (climbLadder)
        {
            if (_playerController.move_forward != 0f)
            {
                playerVelocity.y = moveSpeed * (cameraAngle / 90) * _playerController.move_forward;
            }
            else
            {
                playerVelocity.y = 0;
            }
            if (_playerController.move_right == 0f)
            {
                playerVelocity.x = 0;
                playerVelocity.z = 0;
            }
        }
        else
        {
            playerVelocity.y = 0;
        }

        // walk up stairs
        if (wishSpeed > 0 && stairCatcher.IsColliding())
        {
            Vector3 col = stairCatcher.GetCollisionNormal();
            float ang = Mathf.Rad2Deg(Mathf.Acos(col.Dot(new Vector3(0,1,0))));
            if (ang < maxStairAngle)
            {
                playerVelocity.y = stairJumpHeight;
            }
        }

        if (wishJump)
        {
            playerVelocity.y = jumpSpeed;
            wishJump = false;
        }
    }

    private void ApplyFriction(float t, float delta)
    {
        Vector3 vec = playerVelocity;
        float speed;
        float newspeed;
        float control;
        float drop;

        vec.y = 0.0f;
        speed = vec.Length();
        drop = 0.0f;

        // Only if the player is on the ground then apply friction
        if (touchingGround)
        {
            control = speed < runDeacceleration ? runDeacceleration : speed;
            drop = control * friction * delta * t;
        }

        newspeed = speed - drop;
        if(newspeed < 0)
            newspeed = 0;
        if(speed > 0)
            newspeed /= speed;

        playerVelocity.x *= newspeed;
        playerVelocity.z *= newspeed;
    }

    private void Accelerate(Vector3 wishdir, float wishspeed, float accel, float delta)
    {
        float addspeed;
        float accelspeed;
        float currentspeed;
        
        currentspeed = playerVelocity.Dot(wishdir);
        addspeed = wishspeed - currentspeed;
        if(addspeed <= 0)
            return;
        accelspeed = accel * delta * wishspeed;
        //if(accelspeed > addspeed)
         //   accelspeed = addspeed;
        playerVelocity.x += accelspeed * wishdir.x;
        playerVelocity.z += accelspeed * wishdir.z;

        // implement climbladder?
    }

    /*
    ============
    PM_CmdScale
    Returns the scale factor to apply to cmd movements
    This allows the clients to use axial -127 to 127 values for all directions
    without getting a sqrt(2) distortion in speed.
    ============
    */
     private float CmdScale()
    {
        int max;
        float total;
        float scale;

        max = (int)Mathf.Abs(_playerController.move_forward);
        if(Mathf.Abs(_playerController.move_right) > max)
            max = (int)Mathf.Abs(_playerController.move_right);
        if(max <= 0)
            return 0;

        total = Mathf.Sqrt(_playerController.move_forward * _playerController.move_forward + _playerController.move_right * _playerController.move_right);
        scale = (_tranquilised ? moveSpeed / 2 : moveSpeed) * max / (moveScale * total);

        return scale;
    }

    // ladder
    private void _on_Ladder_body_entered(Player body)
    {
        climbLadder = true;
        // bug? on first load, body entered is called
        if (firstEnter)
        {
            firstEnter = false;
            _on_Ladder_body_exited(body);
        }
    }
    // ladder
    private void _on_Ladder_body_exited(Player body)
    {
        climbLadder = false;
    }
}
