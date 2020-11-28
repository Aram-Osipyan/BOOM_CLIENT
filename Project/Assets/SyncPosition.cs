using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HybridWebSocket;
using System.Text;
using SendModels;

public class SyncPosition : MonoBehaviour
{
    [SerializeField] float timeBetweenSend = 1f;
    private float dTime;
    WebSocket ws;
    void Start()
    {        
        WebSocketInit();
    }

    private void WebSocketInit()
    {
        ws = WebSocketFactory.CreateInstance(SendModels.RemoteSettings.rootUrl);
        ws.OnOpen += () =>
        {
            Debug.Log("WS connected!");
            Debug.Log("WS state: " + ws.GetState().ToString());

            //ws.Send(Encoding.UTF8.GetBytes("Hello from Unity 3D!"));

        };

        // Add OnMessage event listener
        ws.OnMessage += (byte[] msg) =>
        {
            Debug.Log("WS received message: " + Encoding.UTF8.GetString(msg));

            //ws.Close();
        };

        // Add OnError event listener
        ws.OnError += (string errMsg) =>
        {
            Debug.Log("WS error: " + errMsg);
        };

        // Add OnClose event listener
        ws.OnClose += (WebSocketCloseCode code) =>
        {
            Debug.Log("WS closed with code: " + code.ToString());
        };

        // Connect to the server
        ws.Connect();
        SubscribeToChannel();
    }

    private void SubscribeToChannel()
    {
        Channel channel = new Channel("ChatChannel");
        Subscribe sb = new Subscribe("subscribe", JsonUtility.ToJson(channel));
        ws.Send(JsonUtility.ToJson(sb));
    }

    void Update()
    {
        // for sending every timeBetweenSend seconds 
        dTime += Time.deltaTime;
        if (dTime>=timeBetweenSend)
        {
            SendPositionData();
            dTime -= timeBetweenSend;
        }
    }

    private void SendPositionData()
    {
        Data data = new Data();
        data.action = "recieve";
        Channel channel = new Channel("ChatChannel");

        Subscribe sb = new Subscribe("message", JsonUtility.ToJson(channel), JsonUtility.ToJson(data));
        ws.Send(JsonUtility.ToJson(sb));
    }
}
