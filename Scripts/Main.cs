using Godot;
using System;

public class Main : Node
{
    private object[] spawnsTeam1;
    private object[] spawnsTeam2;
    private int currentSpawnTeam1 = 0;
    private int currentSpawnTeam2 = 0;

    public override void _Ready()
    {
        PackedScene playerScene = (PackedScene)ResourceLoader.Load("res://Scenes/Player.tscn");
        Player player = (Player)playerScene.Instance();
        this.AddChild(player);

        PackedScene m = (PackedScene)ResourceLoader.Load("res://Scenes/TeamMenu.tscn");
        TeamMenu m2 = (TeamMenu)m.Instance();
        this.AddChild(m2); 
        m2.Init(player.GetName());
        spawnsTeam1 = GetTree().GetNodesInGroup("SpawnTeam1");
        spawnsTeam2 = GetTree().GetNodesInGroup("SpawnTeam2");
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            Input.SetMouseMode(Input.MouseMode.Visible);
            GetTree().Quit();
        }
    }

    public override void _Input(InputEvent e)
    {
    }

    public Vector3 GetNextSpawn(int teamID)
    {
        Spatial spawn = null;
        switch (teamID)
            {
                case 1:
                    
                    if (spawnsTeam1.Length >= currentSpawnTeam1)
                    {
                        currentSpawnTeam1 = 0;
                    }
                    spawn = (Spatial)spawnsTeam1[currentSpawnTeam1];
                    currentSpawnTeam1++;
                break;
                case 2:
                    
                    if (spawnsTeam2.Length >= currentSpawnTeam2)
                    {
                        currentSpawnTeam2 = 0;
                    }
                    spawn = (Spatial)spawnsTeam2[currentSpawnTeam2];
                    currentSpawnTeam2++;
                break;
                case 9:
                    // nothing for now, just break
                    
                break;
            }
            return teamID == 9 ? new Vector3(0,10,0) : spawn.GetTranslation();
    }
}
