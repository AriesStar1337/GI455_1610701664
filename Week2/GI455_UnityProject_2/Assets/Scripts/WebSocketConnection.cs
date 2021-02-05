using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
public class WebSocketConnection : MonoBehaviour
{
    public string ipAddress;
    public string userName;
    public WebSocket websocket;
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void JoinRoom()
    {
        websocket = new WebSocket(ipAddress);
        websocket.OnMessage += OnMessage;
        websocket.Connect();
        websocket.Send("[" + userName + "] has joined the chat.");
    }

    void OnDestroy()
    {
        if(websocket != null)
        {
            websocket.Close();
        }
    }

    void OnApplicationQuit()
    {
        if(ipAddress != "")
            websocket.Send("[" + userName + "] has left the chat.");
    }

    public void OnMessage(object sender, MessageEventArgs messageEventArgs)
    {
        MessageManager mm = GameObject.Find("MessageManager").GetComponent<MessageManager>();
        mm.CreateText(false, messageEventArgs.Data);
        print(messageEventArgs.Data);
    }
}
