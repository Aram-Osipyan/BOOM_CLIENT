using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HybridWebSocket;
using SendModels;
using UnityEngine;

class SendUtility
{
    public WebSocket webSocket { private set; get; }
    public string channel { get; private set; }
    public Action<byte[]> recieve { get; set; }
    public void ConnectToServer(string rootUrl = SendModels.RemoteSettings.rootUrl)
    {
        webSocket = WebSocketFactory.CreateInstance(rootUrl);
        webSocket.OnOpen += () =>
        {
            Debug.Log("WS connected!");
            Debug.Log("WS state: " + webSocket.GetState().ToString());

        };

        // Add OnMessage event listener
        webSocket.OnMessage += Recieve;

        // Add OnError event listener
        webSocket.OnError += (string errMsg) =>
        {
            Debug.Log("WS error: " + errMsg);
        };

        // Add OnClose event listener
        webSocket.OnClose += (WebSocketCloseCode code) =>
        {
            Debug.Log("WS closed with code: " + code.ToString());
        };

        // Connect to the server
        webSocket.Connect();
    }
    public void SubscribeToChannel(string _channel)
    {
        this.channel = _channel;
        Channel channel = new Channel(_channel);
        Subscribe sb = new Subscribe("subscribe", JsonUtility.ToJson(channel));
        webSocket.Send(JsonUtility.ToJson(sb));
    }
    public void Send(string data)
    {
        webSocket.Send(data);
    }
    public void Send(byte[] data)
    {
        webSocket.Send(data);
    }
    //Debug.Log("WS received message: " + Encoding.UTF8.GetString(msg));
    void Recieve(byte[] msg) => recieve(msg);
    

}

