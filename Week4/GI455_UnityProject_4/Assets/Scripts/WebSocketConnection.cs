using UnityEngine;
using WebSocketSharp;
using UnityEngine.UI;

namespace ChatWebSocket
{
    public class WebSocketConnection : MonoBehaviour
    {
        public struct SocketEvent
        {
            public string eventName;
            public string data;

            public SocketEvent(string eventName, string data)
            {
                this.eventName = eventName;
                this.data = data;
            }
        }
        
        private WebSocket ws;

        private string tempMessageString;

        public delegate void DelegateHandle(SocketEvent result);
        public DelegateHandle OnCreateRoom;
        public DelegateHandle OnJoinRoom;
        public DelegateHandle OnLeaveRoom;
        
        private void Update()
        {
            UpdateNotifyMessage();
        }

        public void Connect(string IP, string Port)
        {
            string url = "ws://" + IP + ":" + Port +"/";

            ws = new WebSocket(url);

            ws.OnMessage += OnMessage;

            ws.Connect();
        }

        public void CreateRoom(InputField roomName)
        {
            SocketEvent socketEvent = new SocketEvent("CreateRoom", roomName.text);

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            ws.Send(toJsonStr);
        }

        public void JoinRoom(InputField roomName)
        {
            SocketEvent socketEvent = new SocketEvent("JoinRoom", roomName.text);

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            ws.Send(toJsonStr);
        }

        public void LeaveRoom()
        {
            SocketEvent socketEvent = new SocketEvent("LeaveRoom", "");

            string toJsonStr = JsonUtility.ToJson(socketEvent);

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

        private void UpdateNotifyMessage()
        {
            if (string.IsNullOrEmpty(tempMessageString) == false)
            {
                SocketEvent receiveMessageData = JsonUtility.FromJson<SocketEvent>(tempMessageString);

                if (receiveMessageData.eventName == "CreateRoom")
                {
                    if (OnCreateRoom != null)
                        OnCreateRoom(receiveMessageData);
                    
                    if(receiveMessageData.data != "fail")
                    {
                        ServerManager.instance.JoinRoom();
                    }
                    else
                    {
                        ServerManager.instance.ErrorReport(0);
                    }
                }
                else if (receiveMessageData.eventName == "JoinRoom")
                {
                    if (OnJoinRoom != null)
                        OnJoinRoom(receiveMessageData);

                    if(receiveMessageData.data != "fail")
                    {
                        ServerManager.instance.JoinRoom();
                    }
                    else
                    {
                        ServerManager.instance.ErrorReport(1);
                    }
                }
                else if(receiveMessageData.eventName == "LeaveRoom")
                {
                    if (OnLeaveRoom != null)
                        OnLeaveRoom(receiveMessageData);
                    ServerManager.instance.LeaveRoom();
                }

                tempMessageString = "";
            }
        }

        private void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            Debug.Log(messageEventArgs.Data);

            tempMessageString = messageEventArgs.Data;
        }
    }
}


