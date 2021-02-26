using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using UnityEngine.UI;

namespace ChatWebSocket
{
    public class WebSocketConnection : MonoBehaviour
    {
        public class rootEvent
        {
            public string eventName;
            public string eventAction;
            public string eventResult;
        }
        public class AccountEvent : rootEvent
        {
            public string userName;
            public string password;
            public string displayName;
        }

        public class RoomEvent : rootEvent
        {
            public string roomName;
        }

        public class ChatEvent : rootEvent
        {
            public string senderID;
            public string senderName;
            public string senderMessage;
        }

        public enum States {NotConnect,Connected,LoggedIn,InRoom};
        public States Status;
        private string userID;
        private WebSocket ws;

        private string tempMessageString;

        public static WebSocketConnection instance;

        public void Awake()
        {
            instance = this;
        }
        
        private void Update()
        {
            UpdateNotifyMessage();
        }

        // Not Connected
        public void Connect()
        {
            string url = "ws://127.0.0.1:8080/";

            ws = new WebSocket(url);

            ws.OnMessage += OnMessage;

            ws.Connect();
        }

        // Connected
        public void SendLoginEvent(string EventType, string Username, string Password, string DisplayName)
        {
            AccountEvent accountData = new AccountEvent();
            accountData.eventName = EventType;
            accountData.userName = Username;
            accountData.password = Password;
            accountData.displayName = DisplayName;
            string toJsonStr = JsonUtility.ToJson(accountData);
            ws.Send(toJsonStr);
        }

        // Logged In
        public void CreateRoom(InputField RoomName)
        {
            if(RoomName.text != "")
                SendRoomEvent("CreateRoom", RoomName.text);
        }

        public void JoinRoom(InputField RoomName)
        {
            if(RoomName.text != "")
                SendRoomEvent("JoinRoom", RoomName.text);
        }

        public void LeaveRoom()
        {
            SendRoomEvent("LeaveRoom", "");
        }

        public void SendRoomEvent(string EventType, string RoomName)
        {
            RoomEvent roomData = new RoomEvent();
            roomData.eventName = EventType;
            roomData.roomName = RoomName;

            string toJsonStr = JsonUtility.ToJson(roomData);
            ws.Send(toJsonStr);
        }

        // InRoom
        public void SendChatEvent(string Message)
        {
            ChatEvent chatData = new ChatEvent();
            chatData.eventName = "Chat";
            chatData.senderMessage = Message;

            string toJsonStr = JsonUtility.ToJson(chatData);
            ws.Send(toJsonStr);
        }

        public void Disconnect()
        {
            if (ws != null)
                ws.Close();
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void OnMessage(object sender, MessageEventArgs recivedMessage)
        {
            Debug.Log(recivedMessage.Data);
            tempMessageString = recivedMessage.Data;
        }

        private void UpdateNotifyMessage()
        {
            if (string.IsNullOrEmpty(tempMessageString) == false)
            {
                rootEvent rootData = JsonUtility.FromJson<rootEvent>(tempMessageString);
                switch (rootData.eventName)
                {
                    case "Connection":
                        if(rootData.eventResult != "fail")
                        {
                            UIManager.instance.ServerConnect();
                            Status = States.InRoom;
                        }
                        else
                        {
                            Disconnect();
                        }
                    break;

                    case "Account":
                        AccountEvent accountData = JsonUtility.FromJson<AccountEvent>(tempMessageString);
                        switch (accountData.eventAction)
                        {
                            case "Login":
                                if(accountData.eventResult != "fail")
                                {
                                    UIManager.instance.Logged(accountData.userName, accountData.displayName);
                                    userID = accountData.userName;
                                }
                                else
                                {
                                    UIManager.instance.ErrorLog(4);
                                }
                            break;

                            case "Regis":
                                if(accountData.eventResult == "fail")
                                {
                                    UIManager.instance.ErrorLog(3);
                                }
                            break;
                        }
                    break;

                    case "Room":
                        RoomEvent roomData = JsonUtility.FromJson<RoomEvent>(tempMessageString);
                        switch(roomData.eventAction)
                        {
                            case "CreateRoom":
                                if(roomData.eventResult != "fail")
                                {
                                    UIManager.instance.JoinRoom(roomData.roomName);
                                }
                                else
                                {
                                    UIManager.instance.ErrorLog(5);
                                }
                            break;

                            case "JoinRoom":
                                if(roomData.eventResult != "fail")
                                {
                                    UIManager.instance.JoinRoom(roomData.roomName);
                                }
                                else
                                {
                                    UIManager.instance.ErrorLog(6);
                                }
                            break;

                            case "LeaveRoom":
                                if(roomData.eventResult != "fail")
                                {
                                    UIManager.instance.LeaveRoom();
                                }
                                else
                                {
                                    UIManager.instance.ErrorLog(7);
                                }
                            break;
                        }
                    break;

                    case "Chat":
                        ChatEvent chatData = JsonUtility.FromJson<ChatEvent>(tempMessageString);
                        MessageManager.instance.CreateText(chatData.senderID == userID, chatData.senderName, chatData.senderMessage);
                    break;
                }
                tempMessageString = "";
            }
        }
    }
}