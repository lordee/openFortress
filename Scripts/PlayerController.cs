using Godot;
using System;
using System.Collections.Generic;

public class PlayerController : Node
{
    // Player commands, stores wish commands that the player asks for (Forward, back, jump, etc)
    public float move_forward = 0;
    public float move_right = 0;
    public float move_up = 0;
    public List<Impulse> Impulses = new List<Impulse>();
    Player _player = null;

    public PlayerController()
    {
        
    }
    public override void _Ready()
    {

    }
    public void Init(Player p)
    {
        _player = p;
    }

    public override void _Input(InputEvent e)
    {
        // moving mouse
        if (Input.GetMouseMode() == Input.MouseMode.Captured)
        {
            if (e is InputEventMouseMotion em)
            {
                if (em.Relative.Length() > 0)
                {          
                    // inverted, fix later
                    _player.RotateHead(-em.Relative.x, em.Relative.y);
                }
            }

            // is this best way to check actions?
            if (Input.IsActionJustPressed("slot1")) 
            {
                Impulses.Add(Impulse.Slot1);
            } 
            else if (Input.IsActionJustPressed("slot2"))
            {
                Impulses.Add(Impulse.Slot2);
            }
            else if (Input.IsActionJustPressed("slot3"))
            {
                Impulses.Add(Impulse.Slot3);
            }
            else if (Input.IsActionJustPressed("slot4"))
            {
                Impulses.Add(Impulse.Slot4);
            }
            if (Input.IsActionJustPressed("detpipe"))
            {
                Impulses.Add(Impulse.Detpipe);
            }
            if (Input.IsActionJustPressed("gren1"))
            {
                Impulses.Add(Impulse.Gren1);
            }
            if (Input.IsActionJustPressed("gren2"))
            {
                Impulses.Add(Impulse.Gren2);
            }
            if (Input.IsActionPressed("attack"))
            {
                Impulses.Add(Impulse.Attack);
            }
            if (Input.IsActionJustPressed("jump"))
            {
                move_up = 1;
            }
            if (Input.IsActionJustReleased("jump"))
            {
                move_up = -1;
            }
            move_forward = 0;
            if (Input.IsActionPressed("move_forward"))
            {
                move_forward += 1;
            }
            if (Input.IsActionPressed("move_backward"))
            {
                move_forward += -1;
            }
            move_right = 0;
            if (Input.IsActionPressed("move_right"))
            {
                move_right += 1;
            }
            if (Input.IsActionPressed("move_left"))
            {
                move_right += -1;
            }
        }
    }

}

public enum Impulse
{
    Slot1
    , Slot2
    , Slot3
    , Slot4
    , Detpipe
    , Gren1
    , Gren2
    , Attack
}