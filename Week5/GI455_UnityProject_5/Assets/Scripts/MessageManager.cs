using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour
{
    public InputField ipfMessage;
    public List<GameObject> textList;
    public GameObject textPrefab;
    public GameObject placeHolder;
    private ChatWebSocket.WebSocketConnection wsc;
    public static MessageManager instance;

    public void Awake()
    {
        instance = this;
    }

    void Start()
    {
        wsc = ChatWebSocket.WebSocketConnection.instance;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SendText();
        }
    }

    public void SendText()
    {
        if(ipfMessage.text != "")
        {
            wsc.SendChatEvent(ipfMessage.text);
            ipfMessage.text = "";
        }
    }
    public void CreateText(bool isSender, string senderName, string Message)
    {
        GameObject tempText = Instantiate(textPrefab, transform.position, transform.rotation);
        textList.Add(tempText);
        tempText.transform.SetParent(placeHolder.transform, false);
        VerticalLayoutGroup VLG = tempText.GetComponent<VerticalLayoutGroup>();
        RectTransform tempRect = tempText.GetComponent<RectTransform>();
        tempText.GetComponentInChildren<Text>().text = Message;
        foreach(Text textChild in tempText.GetComponentsInChildren<Text>())
        {
            if(textChild.gameObject.name == "TextSender")
            {
                textChild.text = " " + senderName + " ";
            }
            if(textChild.gameObject.name == "TextMessage")
            {
                textChild.text = Message;
            }
        }
        tempRect.anchoredPosition = Vector3.zero;
        if(isSender)
        {
            VLG.childAlignment = TextAnchor.UpperRight;
            Image tempImage = tempText.GetComponentInChildren<Image>();
            tempImage.color = new Color32(60, 100, 170, 255);
        }
        tempRect.transform.localPosition = new Vector3(0, textList.Count * -10, 0);
    }

    public void ClearText()
    {
        foreach (var texts in textList)
        {
            Destroy(texts);
        }
    }
}
