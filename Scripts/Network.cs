using Godot;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Linq;

public class Network : Node
{
    // network
    public ConnectionType ConnType;
    public bool Active = false;
    private int _networkID = 0;
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
    UdpClient udp = new UdpClient();
    List<Tuple<int, IPEndPoint>> challenges = new List<Tuple<int, IPEndPoint>>();
    List<OFConnection> connections = new List<OFConnection>();
    List<ClientCommands> unsentCommands = new List<ClientCommands>();
    List<ClientCommands> sentCommands = new List<ClientCommands>();

    List<Tuple<Transform,float>> unsentMovement = new List<Tuple<Transform, float>>();
    List<Tuple<Transform,float>> sentMovement = new List<Tuple<Transform, float>>();

    public override void _Ready()
    {
        
    }

    public override void _Process(float delta)
    {
        if (Active)
        {
            foreach(OFConnection c in connections)
            {
                byte[] packet = BuildPacket(c.NetworkID);
                // check packet is different to last packet
                throw new NotImplementedException();
                if (packet != lastpacket)
                {
                    udp.SendAsync(packet, packet.Length, c.IPAddress);
                }

                // timeout on connections needed
                throw new NotImplementedException();

                // timeout on challenges needed
                throw new NotImplementedException();
            }
        }
    }

    // server hosts
    public void OFServerHost()
    {
        this.ConnType = ConnectionType.Server;
        NetworkID = 1;
        udp.BeginReceive(new AsyncCallback(ReceivePacket), udp);
    }

    // client asks to connect to server
    public void OFClientConnect(string ip, int port)
    {
        // send packet with connect request
        connections.Clear();
        IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
        ConnType = ConnectionType.Client;
        udp.Connect(ep);
        this.OFClientConnectChallenge();
    }

    public void OFClientConnectChallenge()
    {
        udp.BeginReceive(new AsyncCallback(ReceivePacket), udp);
        byte[] cb = Encoding.ASCII.GetBytes("getChallenge");
        udp.SendAsync(cb, cb.Length);
    }

    private void ReceivePacket(IAsyncResult result)
    {
        UdpClient socket = result.AsyncState as UdpClient;
        IPEndPoint source = new IPEndPoint(0, 0);
        // get the actual message and fill out the source
        byte[] bytes = socket.EndReceive(result, ref source);
        socket.BeginReceive(new AsyncCallback(ReceivePacket), socket);
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
                // connection items
                case "getChallenge":
                    // client is challenging server, send response of networkid
                    if (this.ConnType == ConnectionType.Server)
                    {
                        sendChallengeResponse(socket, source);
                    }
                break;
                // received challenge response from server, try to connect
                case "challengeResponse":
                    if (this.ConnType == ConnectionType.Client)
                    {
                        int challID = Convert.ToInt32(msgs[1]);
                        if (NetworkID != challID)
                        {
                            NetworkID = challID;
                            byte[] packet = Encoding.ASCII.GetBytes("connect\n" + NetworkID.ToString());
                            // include state such as name of player
                            throw new NotImplementedException();
                            connections.Add(new OFConnection(1, source));
                            socket.SendAsync(packet, packet.Length);
                        }
                    }
                break;
                // received connect request from client
                case "connect":
                    if (this.ConnType == ConnectionType.Server)
                    {
                        int clientID = Convert.ToInt32(data);
                        // if it exists in challenge list, add to connect list
                        if (challenges.Any(e => e.Item1 == clientID && e.Item2 == source))
                        {
                            if (!connections.Any(e => e.NetworkID == clientID))
                            {
                                Tuple<int, IPEndPoint> t = challenges.First(e => e.Item1 == clientID && e.Item2 == source);
                                connections.Add(new OFConnection(t.Item1, t.Item2));
                                // client will now sync through normal send/receive of packets
                            }
                            challenges.RemoveAll(e => e.Item1 == clientID);
                        }
                        else
                        {
                            // challenge no longer exists, send new challengeResponse
                            sendChallengeResponse(socket, source);
                        }
                    }
                break;
                // need to build ack pn in to connections data, need to track against all clients
                throw new NotImplementedException();
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
    }

    private void sendChallengeResponse(UdpClient socket, IPEndPoint source)
    {
        int chal = challenges.Count > 0 ? challenges.Max(e => e.Item1) : 1;
        int conn = connections.Count > 0 ? connections.Max(e => e.NetworkID) : 1;
        int nextID = chal > conn ? (chal + 1) : (conn + 1);
        
        challenges.Add(new Tuple<int, IPEndPoint>(nextID, source));
        byte[] packet = Encoding.ASCII.GetBytes("challengeResponse\n" + nextID.ToString());
        socket.BeginReceive(new AsyncCallback(ReceivePacket), socket);
        socket.SendAsync(packet, packet.Length, source);
    }

    private byte[] BuildPacket(int clientID)
    {
        byte[] packet = null;

        // build packet
         
        // networkid
        // packetnumber
        
        // acknowledged packetnumber
        // cl_state
        // transform, speed

        // if server then don't send commands, just state
        if (sentMovement.Count > 0)
        {
            // add to packet
        }
        else if (unsentMovement.Count > 0)
        {
            // add to packet

            
            sentMovement.Add(unsentMovement[0]);
            unsentMovement.RemoveAt(0);
        }

        if (this.ConnType == ConnectionType.Client)
        {
            // cl_command
            if (sentCommands.Count > 0)
            {
                // add to packet
            }
            else if (unsentCommands.Count > 0)
            {
                // add to packet


                sentCommands.Add(unsentCommands[0]);
                unsentCommands.RemoveAt(0);
            }
        }
        return packet;
    }

// sending packets
    // clients call this when they want to send a command to the server
    public void AddCommand(ClientCommands c)
    {
        unsentCommands.Add(c);
    }

    public void UpdateMovement(Transform t, float velocity)
    {
        unsentMovement.Add(new Tuple<Transform, float>(t, velocity));
    }
}

public class OFConnection
{
    public int NetworkID;
    public IPEndPoint IPAddress;
    public List<SnapShot> Snapshots = new List<SnapShot>();

    public OFConnection(int networkID, IPEndPoint IP)
    {
        this.NetworkID = networkID;
        this.IPAddress = IP;
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

public enum ConnectionType
{
    Client
    , Server
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
