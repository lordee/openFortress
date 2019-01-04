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
    private int _networkID;
    public int NetworkID {
        get { return _networkID; }
        set 
        { 
            _networkIDBytes = Encoding.ASCII.GetBytes(value.ToString()); 
            _networkID = value;
        }

    }
    private Byte[] _networkIDBytes;

    // client
    int sendPacketNum = 0;
    int _acknowledgedPacketNumber = 0;

    // server
    List<SnapShot> SnapShots = new List<SnapShot>();


    // new networking
    UdpClient udp = null;
    List<Tuple<int, IPEndPoint>> connections = new List<Tuple<int, IPEndPoint>>();
    List<ClientCommands> unsentCommands = new List<ClientCommands>();

    public override void _Ready()
    {
        
    }

    public override void _Process(float delta)
    {
        if (Active)
        {
            UdpClient c = new UdpClient();
            foreach(Tuple<int, IPEndPoint> t in connections)
            {
                byte[] packet = BuildPacket(t.Item1);
                // check packet is different to last packet
                throw new NotImplementedException();
                if (packet != lastpacket)
                {
                    c.Send(packet, packet.Length, t.Item2);
                }
            }
        }
    }

    // client asks to connect to server
    public void OFClientConnect(string ip, int port)
    {
        // send packet with connect request
        connections.Clear();
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
        udp.BeginReceive(new AsyncCallback(OFClientReceivePacket), udp);
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
                NetworkID = Convert.ToInt32(msgs[1]);
                Tuple<int, IPEndPoint> t = new Tuple<int, IPEndPoint>(NetworkID, source);
                connections.Add(t);
                OFClientConnectAttempt();
            break;
            default:
                // for now retry, later we look at error codes
                OFClientConnectChallenge();
            break;
        }
    }

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

            Player p = null;
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
                case "networkID":
                    p = (Player)GetNode("/root/OpenFortress/Main/" + NetworkID.ToString());
                    if (p == null)
                    {
                        // create and add player to scene
                    }
                break;
            }
        }
        socket.BeginReceive(new AsyncCallback(OFClientReceivePacket), socket);
    }

    private byte[] BuildPacket(int clientID)
    {
        byte[] packet = null;

        // build packet
         
        // networkid
        // packetnumber
        // acknowledged packetnumber
        // cl_state

        // if server then don't send commands, just state
        // cl_command
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

    // clients call this when they want to send a command to the server
    public void AddCommand(ClientCommands c)
    {
        unsentCommands.Add(c);
    }
}

public class SnapShot
{
    public int PacketNumber;
    public int ClientID;
    public Transform Transform;
    public SnapShot(int packetNum, int clientID, Transform trans)
    {
        this.PacketNumber = packetNum;
        this.ClientID = clientID;
        this.Transform = trans;
    }
}

public enum ClientCommands
{
    connect
    , disconnect
    , primeOne
    , primeTwo
    , throwGren
    , attack
    , jump
    , moveForward
    , moveLeft
    , moveRight
    , moveBack
}
// packet
    // player join, quit etc commands
    // player chat
    // shoot
    // move

    // packet represents player state
