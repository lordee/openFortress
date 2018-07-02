using Godot;
using System;
using System.Collections.Generic;

struct Cmd 
{
    public float move_forward;
    public float move_right;
    public float move_up;
}

public class Player : KinematicBody
{
    float mouseSensitivity = 0.2f;
    float cameraAngle = 0F;
    // physics
    public float gravity = 20.0f;
    public float friction = 6;
    public float groundSnapTolerance = 1.0f;
    private Vector3 up = new Vector3(0,1,0);
    // stairs
    float maxStairAngle = 20F;
    float stairJumpHeight = 9F;

    // movement
    public float moveSpeed = 15.0f;               // Ground move speed
    public float runAcceleration = 14.0f;         // Ground accel
    public float runDeacceleration = 10.0f;       // Deacceleration that occurs when running on the ground
    public float airAcceleration = 2.0f;          // Air accel
    public float airDecceleration = 2.0f;         // Deacceleration experienced when ooposite strafing
    public float airControl = 0.3f;               // How precise air control is
    public float sideStrafeAcceleration = 50.0f;  // How fast acceleration occurs to get up to sideStrafeSpeed when
    public float sideStrafeSpeed = 1.0f;          // What the max speed to generate when side strafing
    public float jumpSpeed = 8.0f;                // The speed at which the character's up axis gains when hitting jump
    public float moveScale = 1.0f;
   
    private Vector3 moveDirectionNorm = new Vector3();
    private Vector3 playerVelocity = new Vector3();
    
    // Player commands, stores wish commands that the player asks for (Forward, back, jump, etc)
    private Cmd _cmd;

    // Nodes
    Spatial head;
    Camera camera;
    RayCast stairCatcher;
    Sprite3D muzzleFlash;

    // state
    bool climbLadder = false;
    bool firstEnter = true; // bug? on first load, body entered is called

    // shooting
    float shootRange = 1000F;
    Vector2 cameraCenter;
    Vector3 shootOrigin;
    Vector3 shootNormal;
    private bool shooting = false;

    // bunnyhopping
    private bool wishJump = false;
    private bool touchingGround = false;
    private float playerTopVelocity = 0.0f;

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
        head = (Spatial)GetNode("Head");
        camera = (Camera)head.GetNode("Camera");
        stairCatcher = (RayCast)GetNode("StairCatcher");
        muzzleFlash = (Sprite3D)camera.GetNode("MachineGun").GetNode("MuzzleFlash");

        // enable ladders
        var ladders = GetTree().GetNodesInGroup("Ladders");
        foreach (Area l in ladders)
        {
            l.Connect("body_entered", this, "_on_Ladder_body_entered");
            l.Connect("body_exited", this, "_on_Ladder_body_exited");
        }
    }

    public override void _Input(InputEvent e)
    {
        // moving mouse
        if (e is InputEventMouseMotion em)
        {
            if (em.Relative.Length() > 0)
            {          
                head.RotateY(Mathf.Deg2Rad(-em.Relative.x * mouseSensitivity));

                // limit how far up/down we look
                // invert mouse
                float change = em.Relative.y * mouseSensitivity;
                if (cameraAngle + change < 90F && cameraAngle + change > -90F)
                {
                    camera.RotateX(Mathf.Deg2Rad(change));
                    cameraAngle += change;
                }
            }
        }

        // shooting
        if (e is InputEventMouseButton emb && e.IsPressed())
            {
                shootOrigin = camera.ProjectRayOrigin(new Vector2(cameraCenter.x, cameraCenter.y));
                shootNormal = camera.ProjectRayNormal(new Vector2(cameraCenter.x, cameraCenter.y)) * shootRange;

                // TODO change this to a command later
                if (emb.ButtonIndex == 1)
                {
                    shooting = true;
                }
            }
    }

    public override void _PhysicsProcess(float delta)
    {
        QueueJump();
        if (touchingGround)
        {
            GroundMove(delta);
        }
        else
        {
            AirMove(delta);
        }
        playerVelocity = MoveAndSlide(playerVelocity, up);
        touchingGround = IsOnFloor();     
        float speed = playerVelocity.Length();
        //GD.Print("Speed: " + speed.ToString());

        if (shooting)
            {
                muzzleFlash.Show();
                AudioStreamPlayer3D s = (AudioStreamPlayer3D)camera.GetNode("MachineGun").GetNode("Sound");
                s.Play();
                PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;
                // null should be self?
                Dictionary<object, object> result = spaceState.IntersectRay(shootOrigin, shootNormal, new object[] { this }, 1);

                Vector3 impulse;
                Vector3 impact_position;
                if (result.Count > 0)
                {
                    impact_position = (Vector3)result["position"];
                    impulse = (impact_position - (Vector3)GlobalTransform.origin).Normalized();
                    
                    if (result["collider"] is RigidBody c)
                    {
                        Vector3 position = impact_position - c.GlobalTransform.origin;
                        c.ApplyImpulse(position, impulse * 10);
                    }
                }
                
                shooting = false;
            }
            else
            {
                muzzleFlash.Hide();
            }
    }

    private void QueueJump()
    {
        if (Input.IsActionJustPressed("jump") && !wishJump)
        {
            wishJump = true;
        }
        if (Input.IsActionJustReleased("jump"))
        {
            wishJump = false;
        }
    }

    private void SetMovementDir()
    {
        _cmd.move_forward = 0f;
        _cmd.move_right = 0f;
        _cmd.move_forward += Input.IsActionPressed("move_forward") == true ? 1.0f : 0f;
        _cmd.move_forward -= Input.IsActionPressed("move_backward") == true ? 1.0f : 0f;
        _cmd.move_right += Input.IsActionPressed("move_right") == true ? 1.0f : 0f;
        _cmd.move_right -= Input.IsActionPressed("move_left") == true ? 1.0f : 0f;
    }

    private void AirMove(float delta)
    {
        Vector3 wishdir = new Vector3();
        Basis aim = camera.GetGlobalTransform().basis;
        float wishvel = airAcceleration;
        float accel;
        
        SetMovementDir();
        float scale = CmdScale();

        wishdir += aim.x * _cmd.move_right;
        wishdir -= aim.z * _cmd.move_forward;

        float wishspeed = wishdir.Length();
        wishspeed *= moveSpeed;

        wishdir = wishdir.Normalized();
        moveDirectionNorm = wishdir;
        //wishspeed *= scale;

        // CPM: Aircontrol
        float wishspeed2 = wishspeed;
        if (playerVelocity.Dot(wishdir) < 0)
            accel = airDecceleration;
        else
            accel = airAcceleration;
        // If the player is ONLY strafing left or right
        if(_cmd.move_forward == 0 && _cmd.move_right != 0)
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
        if(Mathf.Abs(_cmd.move_forward) < 0.001 || Mathf.Abs(wishspeed) < 0.001)
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

        SetMovementDir();
        float scale = CmdScale();

        wishDir += aim.x * _cmd.move_right;
        wishDir -= aim.z * _cmd.move_forward;
        wishDir = wishDir.Normalized();
        moveDirectionNorm = wishDir;

        float wishSpeed = wishDir.Length();
        wishSpeed *= moveSpeed;
        Accelerate(wishDir, wishSpeed, runAcceleration, delta);

        // Reset the gravity velocity??
        //playerVelocity.y = -gravity * delta;
        
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

        max = (int)Mathf.Abs(_cmd.move_forward);
        if(Mathf.Abs(_cmd.move_right) > max)
            max = (int)Mathf.Abs(_cmd.move_right);
        if(max <= 0)
            return 0;

        total = Mathf.Sqrt(_cmd.move_forward * _cmd.move_forward + _cmd.move_right * _cmd.move_right);
        scale = moveSpeed * max / (moveScale * total);

        return scale;
    }

    // ladder
    public void _on_Ladder_body_entered(Player body)
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
    public void _on_Ladder_body_exited(Player body)
    {
        climbLadder = false;
    }
}
