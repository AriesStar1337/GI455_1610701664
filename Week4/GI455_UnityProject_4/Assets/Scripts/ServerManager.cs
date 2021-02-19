using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ServerManager : MonoBehaviour
{
    public InputField ipIPF;
    public InputField portIPF;
    public InputField roomIPF;
    public GameObject serverUI;
    public GameObject roomUI;
    public GameObject chatUI;
    public Text errorText;
    public Text roomName;
    public ChatWebSocket.WebSocketConnection wsc;
    public static ServerManager instance;

    public void Awake()
    {
        instance = this;
    }

    public void ServerConnect(bool isLocal)
    {
        if(isLocal)
        {
            wsc.Connect("127.0.0.1", "8080");
        }
        else
        {
            wsc.Connect(ipIPF.text, portIPF.text);
        }
        serverUI.SetActive(false);
        roomUI.SetActive(true);
    }

    public void JoinRoom()
    {
        roomUI.SetActive(false);
        chatUI.SetActive(true);
        roomName.text = "Room: " + roomIPF.text;
    }

    public void LeaveRoom()
    {
        chatUI.SetActive(false);
        roomUI.SetActive(true);
        roomName.text = "";
    }

    public void ErrorReport(int ErrorCode)
    {
        switch (ErrorCode)
        {
            case 0: errorText.text = "Room already exist";
            break;
            case 1: errorText.text = "Room doesn't exist";
            break;
        }
    }
}
