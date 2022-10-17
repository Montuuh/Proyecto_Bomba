using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPServer : MonoBehaviour
{
    private int recv;
    private byte[] data = new byte[1024];
    Socket socket;
    EndPoint remote;
    Thread thread;
    int port = 9500;

    private void OnDisable()
    {
        socket.Close();
        thread.Abort();
        StopAllCoroutines();
    }

    // Start is called before the first frame update
    void Start()
    {
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, port);

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.Bind(ipep);
        Debug.Log("Waiting for client...");
        
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, port);
        remote = (EndPoint)(sender);

        // Thread
        thread = new Thread(new ThreadStart(ReceiveData));

        // Data received
        //recv = socket.ReceiveFrom(data, ref remote);
        //Debug.Log("Message received from " + remote.ToString());
        //Debug.Log(Encoding.ASCII.GetString(data, 0, recv));

        // Sends a message to the client
        string welcome = "Welcome to my test server";
        data = Encoding.ASCII.GetBytes(welcome);
        socket.SendTo(data, data.Length, SocketFlags.None, remote);
    }

    // Update is called once per frame
    void Update()
    {
        //recv = socket.ReceiveFrom(data, ref remote);
        //Debug.Log(Encoding.ASCII.GetString(data, 0, recv));
        //socket.SendTo(data, data.Length, SocketFlags.None, remote);
    }

    private void ReceiveData()
    {
        recv = socket.ReceiveFrom(data, ref remote);
        Debug.Log(Encoding.ASCII.GetString(data, 0, recv));
        socket.SendTo(data, data.Length, SocketFlags.None, remote);
    }
}
