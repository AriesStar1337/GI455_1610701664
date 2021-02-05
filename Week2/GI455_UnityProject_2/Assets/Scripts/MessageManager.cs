using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour
{
    public bool test;
    public InputField ipfMessage;
    public List<GameObject> textList;
    public GameObject textPrefab;
    public GameObject placeHolder;
    private WebSocketConnection wsc;

    void Start()
    {
        wsc = GameObject.Find("WebsocketConnection").GetComponent<WebSocketConnection>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            SendText();
        }
    }

    public void SendText()
    {
        if(ipfMessage.text != "")
        {
            CreateText(true, ipfMessage.text);
            wsc.websocket.Send(wsc.userName + ": " + ipfMessage.text);
            print("You: " + ipfMessage.text);
        }
    }
    public void CreateText(bool isSender, string Message)
    {
        GameObject tempText = Instantiate(textPrefab, transform.position, transform.rotation);
        textList.Add(tempText);
        tempText.transform.SetParent(placeHolder.transform, false);
        VerticalLayoutGroup VLG = tempText.GetComponent<VerticalLayoutGroup>();
        RectTransform tempRect = tempText.GetComponent<RectTransform>();
        tempText.GetComponentInChildren<Text>().text = Message;
        tempRect.anchoredPosition = Vector3.zero;
        if(isSender)
        {
            VLG.childAlignment = TextAnchor.UpperRight;
            Image tempImage = tempText.GetComponentInChildren<Image>();
            tempImage.color = new Color32(60, 100, 170, 255);
        }
        tempRect.transform.localPosition = new Vector3(0, textList.Count * -10, 0);
    }
}
