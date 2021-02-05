using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
public class WebSocketConnection : MonoBehaviour
{
    public string ipAddress;
    public string userName;
    private int userId;
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
    }

    void OnDestroy()
    {
        if(websocket != null)
        {
            websocket.Close();
        }
    }

    public void OnMessage(object sender, MessageEventArgs messageEventArgs)
    {
        MessageManager mm = GameObject.Find("MessageManager").GetComponent<MessageManager>();
        mm.CreateText(false, messageEventArgs.Data);
        print(messageEventArgs.Data);
    }
}
