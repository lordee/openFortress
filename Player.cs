using Godot;
using System;
using System.Collections.Generic;

public class Player : KinematicBody
{
    // info
    public int PlayerID;
    public int Team = 0;
    public string playerClass = "";
    bool inMenu = false;

    // move variables
    float camera_angle = 0F;
    Vector2 camera_change = new Vector2();
    float mouse_sensitivity = 0.3F;
    Vector3 velocity = new Vector3();
    Vector3 direction = new Vector3();
    bool climbLadder = false;
    // bug? on first load, body entered is called
    bool firstEnter = true;

    // node references
    RayCast feet; 
    Spatial head;
    Camera camera;
    RayCast stairCatcher;
    Sprite3D muzzleFlash;
            
    float gravity = -9.8F * 3;
    int max_speed = 30;
    int acceleration = 5;
    float MAX_SLOPE_ANGLE = 35F;

    // jumping
    int jump_height = 10;

    // stairs
    float MAX_STAIR_ANGLE = 20F;
    float STAIR_JUMP_HEIGHT = 9F;


    
    // shooting
    float shoot_range = 1000F;
    Vector2 cameraCenter;
    Vector3 shoot_origin;
    Vector3 shoot_normal;
    float shooting = 0F;

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
        
        feet = (RayCast)GetNode("Feet");
        head = (Spatial)GetNode("Head");
        camera = (Camera)head.GetNode("Camera");
        stairCatcher = (RayCast)GetNode("StairCatcher");
        muzzleFlash = (Sprite3D)camera.GetNode("MachineGun").GetNode("MuzzleFlash");
        
        cameraCenter.x = OS.GetWindowSize().x / 2;
        cameraCenter.y = OS.GetWindowSize().y / 2;

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
        if (!inMenu)
        {
            if (e is InputEventMouseMotion em)
            {
                camera_change = em.Relative;
            }
            if (e is InputEventMouseButton emb && e.IsPressed())
            {
                shoot_origin = camera.ProjectRayOrigin(new Vector2(cameraCenter.x, cameraCenter.y));
                shoot_normal = camera.ProjectRayNormal(new Vector2(cameraCenter.x, cameraCenter.y)) * shoot_range;

                // two styles of shooting
                if (emb.ButtonIndex == 1)
                {
                    shooting = 1;
                }
            }
        }
    }
    public override void _PhysicsProcess(float delta)
    {
        if (!inMenu)
        {
            Aim();
            Move(delta);
            
            if (shooting > 0)
            {
                muzzleFlash.Show();
                AudioStreamPlayer3D s = (AudioStreamPlayer3D)camera.GetNode("MachineGun").GetNode("Sound");
                s.Play();
                PhysicsDirectSpaceState space_state = GetWorld().DirectSpaceState;
                // null should be self?
                Dictionary<object, object> result = space_state.IntersectRay(shoot_origin, shoot_normal, new object[] { this }, 1);

                Vector3 impulse;
                Vector3 impact_position;
                if (result.Count > 0)
                {
                    impact_position = (Vector3)result["position"];
                    impulse = (impact_position - (Vector3)GlobalTransform.origin).Normalized();
                    
                    if (result["collider"] is RigidBody c)
                    {
                        Vector3 position = impact_position - c.GlobalTransform.origin;
                        if (shooting == 1)
                        {
                            c.ApplyImpulse(position, impulse * 10);
                        }
                    }
                }
                
                shooting = 0;
            }
            else
            {
                muzzleFlash.Hide();
            }
        }
    }

    public void SetInMenu(bool inmenu)
    {
        inMenu = inmenu;
        if (inmenu)
        {
            Input.SetMouseMode(Input.MouseMode.Visible);
        }
        else
        {
            Input.SetMouseMode(Input.MouseMode.Captured);
        }
    }
    public void Move(float delta)
    {
        // reset the direction of the player
        direction = new Vector3();

        // get the rotation of the camera
        Basis aim = camera.GetGlobalTransform().basis;
                
        if (Input.IsActionPressed("move_forward"))
        {
            direction -= aim.z;
        }
        if (Input.IsActionPressed("move_backward"))
        {
            direction += aim.z;
        }
        if (Input.IsActionPressed("move_left"))
        {
            direction -= aim.x;
        }
        if (Input.IsActionPressed("move_right"))
        {
            direction += aim.x;
        }
        
        // don't slide down all ramps
        if (IsOnFloor())
        {
            Vector3 n = feet.GetCollisionNormal();
            float floorAngle = Mathf.Rad2Deg(Mathf.Acos(n.Dot(new Vector3(0,1,0))));
            if (floorAngle > MAX_SLOPE_ANGLE)
            {
                velocity.y += gravity * delta;
            }
        }
        else
        {
            // in the air, apply gravity
            velocity.y += gravity * delta;
        }
      
        Vector3 temp_velocity = velocity;
        // if they're not climbing a ladder, then they walk on the floor, not upwards
        if (!climbLadder)
        {
            direction.y = 0;           
            temp_velocity.y = 0;
        }
        direction = direction.Normalized();
        
        // walk stairs
        if (direction.Length() > 0 && stairCatcher.IsColliding())
        {
            Vector3 col = stairCatcher.GetCollisionNormal();
            float ang = Mathf.Rad2Deg(Mathf.Acos(col.Dot(new Vector3(0,1,0))));
            if (ang < MAX_STAIR_ANGLE)
            {
                velocity.y = STAIR_JUMP_HEIGHT;
            }
        }

        // where would the player go at max speed
        Vector3 target = direction * max_speed;

        // calculate a portion of the distance to go
        temp_velocity = temp_velocity.LinearInterpolate(target, acceleration * delta);
        velocity.x = temp_velocity.x;
        velocity.z = temp_velocity.z;
        
        // move
        if (climbLadder)
        {
            velocity.y = temp_velocity.y;
            velocity = MoveAndSlide(velocity);
        }
        else
        {
            velocity = MoveAndSlide(velocity, new Vector3(0,1,0));
        }
        
        // jump
        if (IsOnFloor() && Input.IsActionJustPressed("jump"))
        {
            velocity.y = jump_height;
        }

        // make sure staircatcher always in direction walking
        stairCatcher.SetTranslation(new Vector3(direction.x, stairCatcher.Translation.y, direction.z));
    }
    public void Aim()
    {
        if (camera_change.Length() > 0)
        {          
            head.RotateY(Mathf.Deg2Rad(-camera_change.x * mouse_sensitivity));

            // limit how far up/down we look
            float change = -camera_change.y * mouse_sensitivity;
            if (camera_angle + change < 90F && camera_angle + change > -90F)
            {
                // invert mouse
                camera.RotateX(Mathf.Deg2Rad(-change));
                camera_angle += change;
                camera_change = new Vector2();
            }
        }
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
