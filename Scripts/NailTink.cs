using Godot;
using System;

public class NailTink : Particles
{
    public override void _Ready()
    {
    }

    public override void _Process(float delta)
    {
        if (!this.IsEmitting())
        {
            GetTree().QueueDelete(this);
        }
    }
}
