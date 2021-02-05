using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoomConnection : MonoBehaviour
{
    public InputField ipIPF;
    public InputField portIPF;
    public InputField nameIPF;
    public WebSocketConnection wsc;

    public void ServerConnect()
    {
        wsc.ipAddress = "ws://" + ipIPF.text + ":" + portIPF.text + "/";
        LoadChat();
    }

    public void LocalConnect()
    {
        wsc.ipAddress = "ws://127.0.0.1:25500/";
        LoadChat();
    }

    private void LoadChat()
    {
        wsc.userName = nameIPF.text;
        wsc.JoinRoom();
        SceneManager.LoadScene(1);
    }
}
