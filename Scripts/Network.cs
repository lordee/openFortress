using Godot;
using System;
using System.Collections.Generic;

public class Network : Node
{
    // network
    public bool Active = false;
    public int NetworkID;

    // client
    int sendPacketNum = 0;
    int _acknowledgedPacketNumber = 0;
    List<Vector3> playerTranslations = new List<Vector3>();
    List<Tuple<int, Vector3>> playerTranslationsSent = new List<Tuple<int, Vector3>>();
    Vector3 lastTranslation = new Vector3();

    // server
    List<SnapShot> ClientSnapShots = new List<SnapShot>();
    List<int> ConnectedClients = new List<int>();


    public override void _Ready()
    {
        
    }

    public override void _Process(float delta)
    {
        if (Active)
        {
            // check if there is new data
            object[] packet = null;

            packet = GetPacket(this.NetworkID, 0);

            // send to server
            if (packet != null)
            {
                RpcUnreliableId(1, "ReceivePacket", packet);
            }
        }
    }

    private object[] GetPacket(int clientID, int otherClientID)
    {
        object[] packet = null;
        int packetNum = 0;
        Vector3 trans = new Vector3();
        // if there is a command not yet sent or acknowledged, add to packet
        sendPacketNum++;
        packetNum = sendPacketNum;
        
        if (playerTranslationsSent.Count > 0) // unacknowledged sent translations
        {
            trans = playerTranslationsSent[0].Item2;
            packet = new object[] { packetNum, clientID, otherClientID, trans };
        }
        else if (playerTranslations.Count > 0)
        {               
            trans = playerTranslations[0];

            playerTranslationsSent.Add(new Tuple<int, Vector3>(sendPacketNum, trans));
            playerTranslations.RemoveAt(0);
            packet = new object[] { packetNum, clientID, otherClientID, trans };
        }

        return packet;
    }

    [Remote]
    public void ReceivePacket(int packetNum, int clientID, int otherClientID, Vector3 trans)
    {
        // if client receives packet, check packet number in response, get rid of commands in history from this, stop resending
        if (packetNum > _acknowledgedPacketNumber)
        {
            _acknowledgedPacketNumber = packetNum;
            playerTranslationsSent.RemoveAll(e => e.Item1 <= _acknowledgedPacketNumber);
        }
        // move other client
        if (otherClientID != 0)
        {
            MovePlayer(otherClientID, trans);
        }
    }

// sending packets
    public void SpawnPlayer(Player p)
    {
        // add spawn player to gamestate
        Vector3 pt = p.Translation;
        playerTranslations.Add(pt);
    }

    public void UpdateTranslation(Vector3 trans)
    {
        if (!playerTranslations.Contains(trans) && trans != lastTranslation)
        {
            playerTranslations.Add(trans);
            lastTranslation = trans;
        }
    }

// receiving packets
    private void MovePlayer(int clientID, Vector3 trans)
    {
        Player p = (Player)GetNode("/root/OpenFortress/Main/" + clientID.ToString());
        p.Translation = trans;
        GD.Print (p.Translation);
    }
}

public class SnapShot
{
    public int PacketNumber;
    public int ClientID;
    public Vector3 Translation;
    public SnapShot(int packetNum, int clientID, Vector3 trans)
    {
        this.PacketNumber = packetNum;
        this.ClientID = clientID;
        this.Translation = trans;
    }
}
// packet
    // player join, quit etc commands
    // player chat
    // shoot
    // move

    // packet represents player state
