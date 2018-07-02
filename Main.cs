using Godot;
using System;

public class Main : Node
{
    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";

    public override void _Ready()
    {
        Input.SetMouseMode(Input.MouseMode.Captured);

    }

    public override void _Process(float delta)
    {
        // Called every frame. Delta is time since last frame.
        // Update game logic here.
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            Input.SetMouseMode(Input.MouseMode.Visible);
            GetTree().Quit();
        }
    }

    public override void _Input(InputEvent e)
    {
    }
}
