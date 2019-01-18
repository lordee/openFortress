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

    // new networking
    UdpClient udp = new UdpClient();
    List<Tuple<int, IPEndPoint>> challenges = new List<Tuple<int, IPEndPoint>>();
    List<OFConnection> connections = new List<OFConnection>();

    public override void _Ready()
    {
    }

    public override void _Process(float delta)
    {
        if (Active)
        {
            foreach(OFConnection receiver in connections)
            {
                foreach(OFConnection send in connections)
                {
                    if (this.ConnType == ConnectionType.Server || receiver.NetworkID != send.NetworkID)
                    {
                        byte[] packet = BuildPacket(send);

                        udp.SendAsync(packet, packet.Length, receiver.IPAddress);
                    }
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

                                throw new NotImplementedException();
                                // send client initial state for themselves (location of spawn etc)

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
                case "networkID":
                    int id = Convert.ToInt32(data);
                    if (connections.Any(c => c.NetworkID == id))
                    {
                        DecodePacket(msgs, id);
                    }
                break;
            }
        }
    }

    private void DecodePacket(string[] msgs, int id)
    {
        OFConnection peer = connections.First(c => c.NetworkID == id);
        SnapShot ss = new SnapShot();
        ss.PacketData = msgs;
        Player p = null;

        for (int i = 0; i < msgs.Length; i++)
        {
            string type = msgs[i];
            i++;
            string data = msgs[i];
            
            switch (type)
            {
                case "packetNumber":
                    ss.PacketNumber = Convert.ToInt32(data);
                    if (ss.PacketNumber <= peer.AcknowledgedPacketNumber)
                    {
                        return;
                    }
                    else
                    {
                        peer.AcknowledgedPacketNumber = ss.PacketNumber;
                    }
                break;
                case "clientID":
                    ss.ClientID = Convert.ToInt32(data);
                    p = (Player)GetNode("/root/openFortress/" + data);

                    if (p == null)
                    {
                        // spawn a player
                        throw new NotImplementedException();
                    }
                break;
                case "transform":
                    float[] vecs = data.Split(",").Select(float.Parse).ToArray();
                    Vector3 x = new Vector3(vecs[0], vecs[1], vecs[2]);
                    Vector3 y = new Vector3(vecs[3], vecs[4], vecs[5]);
                    Vector3 z = new Vector3(vecs[6], vecs[7], vecs[8]);
                    Vector3 org = new Vector3(vecs[9], vecs[10], vecs[11]);

                    // for now find player and apply, later we need to put on all ents
                    p.PlayerController.Transform = new Transform(x, y, z, org);
                break;
                case "velocity":
                    throw new NotImplementedException();
                break;
                case "move_forward":
                    p.PlayerController.move_forward = (float)Convert.ToInt16(data);
                break;
                case "move_right":
                    p.PlayerController.move_right = (float)Convert.ToInt16(data);
                break;
                case "impulses":
                    int[] imps = data.Split(",").Select(Int32.Parse).ToArray();
                    foreach (Impulse imp in imps)
                    {
                        p.PlayerController.Impulses.Add(imp);
                    }
                break;
            }
        }
    }

    private byte[] BuildPacket(OFConnection client)
    {
        byte[] packet = null;
        

        throw new NotImplementedException();
        // build packet
         
        // networkid
        // packetnumber
        
        // acknowledged packetnumber
        // cl_state
        // transform, speed

        return packet;
    }
}

public class OFConnection
{
    public int NetworkID;
    public IPEndPoint IPAddress;
    public int AcknowledgedPacketNumber;
    public List<SnapShot> Snapshots = new List<SnapShot>();
    
    List<ClientCommands> unsentCommands = new List<ClientCommands>();
    List<ClientCommands> sentCommands = new List<ClientCommands>();
    List<Tuple<Transform,float>> unsentMovement = new List<Tuple<Transform, float>>();
    List<Tuple<Transform,float>> sentMovement = new List<Tuple<Transform, float>>();


    public OFConnection(int networkID, IPEndPoint IP)
    {
        this.NetworkID = networkID;
        this.IPAddress = IP;
    }
}
public struct SnapShot
{
    public int PacketNumber;
    public int ClientID;
    public string[] PacketData;
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
