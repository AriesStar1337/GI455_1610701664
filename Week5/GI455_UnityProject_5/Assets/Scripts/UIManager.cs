using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // GameObject
    public GameObject pageServer;
    public GameObject pageLogin;
    public GameObject pageRoom;
    public GameObject pageChat;

    // InputField
    public InputField ipfNickName;
    public InputField ipfUserName;
    public InputField ipfPass;
    public InputField ipfRepass;
    public InputField ipfRoomName;

    // Text
    public Text textLog;
    public Text textNameRoom;
    public Text[] textUserName;

    // ETC
    public Animator anim;
    public ChatWebSocket.WebSocketConnection wsc;
    public static UIManager instance;
    private string userDisplay;

    public void Awake()
    {
        instance = this;
    }

    public void ServerConnect()
    {
        anim.SetBool("isConnected", true);
        MessageLog(0);
    }

    public void Registing(bool isRegisting)
    {
        anim.SetBool("isRegisting", isRegisting);
    }

    public void Register()
    {
        if(ipfNickName.text == "" || ipfUserName.text == "" || ipfPass.text == "" || ipfRepass.text == "")
        {
            ErrorLog(0);
        }
        else if(ipfUserName.text.Length < 6 || ipfPass.text.Length < 6)
        {
            ErrorLog(1);
        }
        else if(ipfPass.text != ipfRepass.text)
        {
            ErrorLog(2);
        }
        else
        {
            wsc.SendLoginEvent("Regis", ipfUserName.text, ipfPass.text, ipfNickName.text);
        }
    }

    public void LogIn()
    {
        wsc.SendLoginEvent("Login", ipfUserName.text, ipfPass.text, "");
    }

    public void Logged(string UserName, string DisplayName)
    {
        userDisplay = DisplayName;
        foreach (var textDisplay in textUserName)
        {
            textDisplay.text = "User: " + UserName + " (" + DisplayName + ")";
        }
        anim.SetBool("isLogged", true);
        MessageLog(1);
    }

    public void JoinRoom(string RoomName)
    {
        anim.SetBool("isChatting", true);
        textNameRoom.text = "Room: " + RoomName;
        MessageLog(2);
    }

    public void LeaveRoom()
    {
        anim.SetBool("isChatting", false);
        MessageManager.instance.ClearText();
        MessageLog(3);
    }

    public void MessageLog(int LogID)
    {
        textLog.color = Color.green;
        switch (LogID)
        {
            // Server
            case 0: textLog.text = "Connected";
            break;

            // Login
            case 1: textLog.text = "Logged In";
            break;

            // Room
            case 2: textLog.text = "Room joined";
            break;
            case 3: textLog.text = "Room left";
            break;
        }
        anim.SetTrigger("showLog");
    }

    public void ErrorLog(int ErrorCode)
    {
        textLog.color = Color.red;
        switch (ErrorCode)
        {
            // Register
            case 0: textLog.text = "Input empty";
            break;
            case 1: textLog.text = "Username / Password too short";
            break;
            case 2: textLog.text = "Password doesn't match";
            break;
            case 3: textLog.text = "Username already exist";
            break;

            //  Login
            case 4: textLog.text = "Login fail";
            break;

            // Room
            case 5: textLog.text = "Room already exist";
            break;
            case 6: textLog.text = "Room doesn't exist";
            break;
            case 7: textLog.text = "Leave room fail";
            break;
        }
        anim.SetTrigger("showLog");
    }
}
