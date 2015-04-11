using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

public class KinectInteractionClient
{
    public enum InteractionHandEventType
    {
        None = 0,
        Grip,
        GripRelease
    }

    private TcpClient client;

    private String Host = "127.0.0.1";
    private Int32 Port = 8888;

    private Thread thread;
    private NetworkStream stream;

    private InteractionHandEventType lastHandEventType = InteractionHandEventType.None;
    private InteractionHandEventType currhandEventType = InteractionHandEventType.None;

    public Vector2 handPosition = new Vector2(0, 0);

    public void Run(int handType)
    {
        try
        {
            // Initialize client and its stream
            client = new TcpClient(Host, Port);
            stream = client.GetStream();

            // Send to server the hand type
            stream.WriteByte((Byte)handType);

            // Start thread
            thread = new Thread(new ThreadStart(HandleClientCommunication));
            thread.Start();

        }
        catch (Exception e)
        {
            Debug.Log("Error : " + e);
            this.Close();
        }
    }

    private void HandleClientCommunication()
    {
        while (true)
        {
            BinaryReader br = null;
            try
            {
                br = new BinaryReader(stream);
                
                // Receive informations in the order that they've been sent
                currhandEventType = (InteractionHandEventType)br.ReadInt32();
                handPosition.x = (float)br.ReadDouble();
                handPosition.y = (float)br.ReadDouble();
                
                // Don't take the interaction event "None"
                if (currhandEventType != InteractionHandEventType.None)
                    lastHandEventType = currhandEventType;

            }
            catch (Exception e)
            {
                Debug.Log("Error : " + e);
                this.Close();
                break;
            }

            if (br == null)
            {
                // Client has disconnected
                Debug.Log("Disconnected");
                this.Close();
                break;
            }
        }
    }

    public bool HandIsClose()
    {
        if (lastHandEventType == InteractionHandEventType.Grip)
            return true;

        return false;
    }

    public bool HandIsOpen()
    {
        if (lastHandEventType == InteractionHandEventType.GripRelease)
            return true;

        return false;
    }

    public void Close()
    {
        stream.Close();
        client.Close();
    }
}