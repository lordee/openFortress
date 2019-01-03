using Godot;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;

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

    UdpClient udp = null;


    public override void _Ready()
    {
        
    }

    public override void _Process(float delta)
    {
        if (Active)
        {
            // if client, build packet for one client only

            // if server, build packet of all changed ents and communicate to all players
            
            
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

    // client asks to connect to server
    public void OFClientConnect(string ip, int port)
    {
        // send packet with connect request
        udp = new UdpClient();
        IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
        udp.Connect(ep);
        this.OFClientConnectChallenge();
    }

    public void OFClientConnectChallenge()
    {
        udp.BeginReceive(new AsyncCallback(OFClientReceiveConnectAck), udp);
        byte[] cb = Encoding.ASCII.GetBytes("getChallenge");
        udp.Send(cb, cb.Length);
    }
    public void OFClientConnectAttempt()
    {
        udp.BeginReceive(new AsyncCallback(OFClientReceiveConnectAck), udp);
        byte[] cb = Encoding.ASCII.GetBytes("connect\n" + NetworkID.ToString());
        // add conn string info
        udp.Send(cb, cb.Length);
    }

    private void OFClientReceiveConnectAck(IAsyncResult result)
    {
        UdpClient socket = result.AsyncState as UdpClient;
        IPEndPoint source = new IPEndPoint(0, 0);
        // get the actual message and fill out the source
        byte[] message = socket.EndReceive(result, ref source);
        string msg = Encoding.ASCII.GetString(message);

        Console.WriteLine("Got '" + msg + " from " + source);

        string[] msgs = msg.Split("\n");
 
        // if connected, get connection id
        switch (msgs[0])
        {
            case "challengeResponse":
                // schedule the next receive operation once reading is done
                socket.BeginReceive(new AsyncCallback(OFClientReceivePacket), socket);
                NetworkID = Convert.ToInt32(msgs[1]);
                OFClientConnectAttempt();
            break;
            default:
                // for now retry, later we look at error codes
                OFClientConnectChallenge();
            break;
        }
    }

    //public void ReceivePacket(int packetNum, int clientID, int otherClientID, Vector3 trans)
    private void OFClientReceivePacket(IAsyncResult result)
    {
        UdpClient socket = result.AsyncState as UdpClient;
        IPEndPoint source = new IPEndPoint(0, 0);
        // get the actual message and fill out the source
        byte[] bytes = socket.EndReceive(result, ref source);
        string stringbytes = Encoding.ASCII.GetString(bytes);

        Console.WriteLine("Got '" + stringbytes + " from " + source);

        string[] msgs = stringbytes.Split("\n");

        for (int i = 0; i < msgs.Length; i++)
        {
            string type = msgs[i];
            i++;
            string data = msgs[i];
            switch (type)
            {
                // if client receives packet, check packet number in response, get rid of commands in history from this, stop resending
                case "packetNum":
                    int pn = Convert.ToInt32(data);
                    if (pn < _acknowledgedPacketNumber)
                    {
                        // data is old, stop inspecting packet
                        return;
                    }
                    else
                    {
                        _acknowledgedPacketNumber = pn;
                        // remove old packet data from queue
                        throw new NotImplementedException();
                    }
                break;
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
