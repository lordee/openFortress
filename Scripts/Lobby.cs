using Godot;
using System;

public class Lobby : Control
{
    int DEFAULT_PORT = 8910; // some random number, pick your port properly

    Button _joinBtn;
    Button _hostBtn;
    Network _network;
    public override void _Ready()
    {
        _joinBtn = (Button)GetNode("panel/join");
        _hostBtn  = (Button)GetNode("panel/host");
        _hostBtn.Connect("pressed", this, "_On_Host_Pressed");
        _joinBtn.Connect("pressed", this, "_On_Join_Pressed");

        // connect all the callbacks related to networking
        GetTree().Connect("network_peer_connected", this, "_Player_Connected");
        GetTree().Connect("network_peer_disconnected", this, "_Player_Disconnected");
        GetTree().Connect("connected_to_server", this, "_Connected_OK");
        GetTree().Connect("connection_failed", this, "_Connected_Fail");
        GetTree().Connect("server_disconnected", this, "_Server_Disconnected");
    }

    // Network callbacks from SceneTree

    // callback from SceneTree
    private void _Player_Connected(int id)
    {
        GD.Print("player connected");
        // someone connected, start the game!
	    PackedScene main = (PackedScene)ResourceLoader.Load("res://Scenes/Main.tscn");
        Main inst = (Main)main.Instance();
        Node of = GetNode("/root/OpenFortress");

        of.AddChild(inst);
	
        _network = (Network)GetNode("/root/OpenFortress/Network");
        _network.NetworkID = GetTree().GetNetworkUniqueId();
        _network.Active = true;
        this.Hide();
    }

    private void _Player_Disconnected(int id)
    {
        if (GetTree().IsNetworkServer())
        {
            _End_Game("Client disconnected");
        }
        else
        {
            _End_Game("Server disconnected");
        }
    }
    
    // callback from SceneTree, only for clients (not server)
    private void _Connected_OK()
    {
        GD.Print("connected to server");
        _network = (Network)GetNode("/root/OpenFortress/Network");
        _network.Active = true;
        // will not use this one
        return;   
    }

    // callback from SceneTree, only for clients (not server)	
    private void _Connected_Fail()
    {
        _Set_Status("Couldn't connect",false);
	
        // remove peer
        GetTree().SetNetworkPeer(null);
        
        _joinBtn.SetDisabled(false);
        _hostBtn.SetDisabled(false);
    }

    private void _Server_Disconnected()
    {
        _End_Game("Server disconnected");
    }
	
    // Game creation functions

    private void _End_Game(string with_error)
    {     
        // remove peer
        GetTree().SetNetworkPeer(null);

        _joinBtn.SetDisabled(false);
        _hostBtn.SetDisabled(false);
        _network.Active = false;
               
        _Set_Status(with_error, false);
    }
	
    private void _Set_Status(string text, bool isok)
    {
        // simple way to show status		
        Label ok = (Label)GetNode("panel/status_ok");
        Label fail = (Label)GetNode("panel/status_fail");
        if (isok)
        {
            ok.Text = text;
            fail.Text = "";
        }
        else
        {
            ok.Text = "";
            fail.Text = text;
        }      
    }

    private void _On_Host_Pressed()
    {
        GD.Print("on host pressed");
        NetworkedMultiplayerENet host = new NetworkedMultiplayerENet();
        host.SetCompressionMode(NetworkedMultiplayerENet.CompressionModeEnum.RangeCoder);
        // max: 1 peer, since it's a 2 player game for now
        Error err = host.CreateServer(DEFAULT_PORT, 1);

        if (err != Error.Ok)
        {
            _Set_Status("Can't host, address in use.",false);
            return;
        }
        
        GetTree().SetNetworkPeer(host);
        _joinBtn.SetDisabled(true);
        _hostBtn.SetDisabled(true);
        _Set_Status("Waiting for player..", true);
    }
	
    private void _On_Join_Pressed()
    {
        LineEdit add = (LineEdit)GetNode("panel/address");
        string ip = add.Text;

        if (!ip.IsValidIpAddress())
        {
            _Set_Status("IP address is invalid", false);
            return;
        }

        _network = (Network)GetNode("/root/OpenFortress/Network");
        _network.OFConnect(ip, DEFAULT_PORT);
        
        _Set_Status("Connecting..", true);
    }
}