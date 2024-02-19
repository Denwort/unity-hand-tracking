using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPRecieveGestures : MonoBehaviour
{
    Thread receiveThread;
    UdpClient client;
    public int port;
    public bool startRecieving = true;
    public bool printToConsole = false;
    public string data;

    public static string mydata;

    void Start()
    {
        mydata = "";
        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = false;
        receiveThread.Start();
        
    }


    // receive thread
    private void ReceiveData()
    {

        // Recieve data via udp and converts it to string
        client = new UdpClient(port);
        while (startRecieving)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] dataByte = client.Receive(ref anyIP);
                data = Encoding.UTF8.GetString(dataByte);
                mydata = data;

                if (printToConsole) { print(data); }
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

}
