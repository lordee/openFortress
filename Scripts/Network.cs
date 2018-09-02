using Godot;
using System;
using System.Collections.Generic;

public class Network : Node
{
    // network
    public bool Active = false;

    // client
    int sendPacketNum = 0;
    int _acknowledgedPacketNumber;
    List<Vector3> playerTranslations = new List<Vector3>();
    List<Tuple<int, Vector3>> playerTranslationsSent = new List<Tuple<int, Vector3>>();

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

            if (this.IsNetworkMaster())
            {
                // send data to each client
                // we may want to just send all game data to all clients to reduce complexity in early days
                foreach (int clientID in ConnectedClients)
                {
                    packet = GetPacket(clientID);

                    if (packet != null)
                    {
                        RpcUnreliableId(clientID, "ReceivePacket", packet);
                    }
                }
            }
            else
            {
                packet = GetPacket(GetTree().GetNetworkUniqueId());

                // send to server
                if (packet != null)
                {
                    RpcUnreliableId(1, "ReceivePacket", packet);
                }
            }
        }
    }

    private object[] GetPacket(int clientID)
    {
        int packetNum = 0;
        Vector3 trans = new Vector3();
        //object[] packet = null;
        if (this.IsNetworkMaster())
        {
            // check if other clients have sent updated data
            // check if there are changes to projectiles or other ents
            //p = new Packet();
            //p.PacketNumber = ClientSnapShots.FindLast(ss => ss.Item1 == clientID).Item2.PacketNumber;
        }
        else
        {
            // if there is a command not yet sent or acknowledged, add to packet
            sendPacketNum++;
            packetNum = sendPacketNum;
            

            if (playerTranslationsSent.Count > 0)
            {
                trans = playerTranslationsSent[0].Item2;
            }
            else if (playerTranslations.Count > 0)
            {               
                trans = playerTranslations[0];

                playerTranslationsSent.Add(new Tuple<int, Vector3>(sendPacketNum, trans));
                playerTranslations.RemoveAt(0);
            }
        }

        object[] packet = { packetNum, clientID, trans };
        return packet;
    }

    public void AddClient(int id)
    {
        ConnectedClients.Add(id);
        SnapShot ss = new SnapShot(0, id, new Vector3());
        ClientSnapShots.Add(ss);
    }

    [Remote]
    public void ReceivePacket(int packetNum, int clientID, Vector3 trans)
    {
        GD.Print("Got packet");
        if (this.IsNetworkMaster())
        {
            // if server receives packet, put changes in, update response packet with packetnumber
            SnapShot lastPacket = ClientSnapShots.FindLast(ss => ss.ClientID == clientID);
            if (packetNum > lastPacket.PacketNumber)
            {
                // find player, if not exist create at translation
                Main main = (Main)GetNode("/root/OpenFortress/Main");
                if (!main.HasNode(clientID.ToString()))
                {
                    main.AddPlayer(false, clientID);
                }

                Player p = (Player)GetNode("/root/OpenFortress/Main/" + clientID.ToString());
                p.Translation = trans;

                // add new snapshot
                if (ClientSnapShots.FindAll(ss => ss.ClientID == clientID).Count > 32)
                {
                    ClientSnapShots.Remove(ClientSnapShots.Find(ss => ss.ClientID == clientID));
                }
                ClientSnapShots.Add(new SnapShot(packetNum, clientID, trans));
            }
        }
        else
        {
            // if client receives packet, check packet number in response, get rid of commands in history from this, stop resending
            if (packetNum > _acknowledgedPacketNumber)
            {
                _acknowledgedPacketNumber = packetNum;
                playerTranslationsSent.RemoveAll(e => e.Item1 <= _acknowledgedPacketNumber);
            }
        }
    }

    public void SpawnPlayer(Player p)
    {
        // add spawn player to gamestate
        if (!this.IsNetworkMaster())
        {
            Vector3 pt = p.Translation;
            playerTranslations.Add(pt);
        }
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
