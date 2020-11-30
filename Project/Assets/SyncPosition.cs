using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HybridWebSocket;
using System.Text;
using SendModels;

public class SyncPosition : MonoBehaviour
{
    /// <summary>
    /// Time in seconds
    /// </summary>
    [SerializeField] float timeBetweenSend = 1f;
    private float dTime;
    SendUtility sendUtility;
    void Start()
    {        
        WebSocketInit();
    }

    private void WebSocketInit()
    {
        sendUtility = new SendUtility();
        sendUtility.ConnectToServer();
        sendUtility.SubscribeToChannel("PlayerPositionSyncChannel");
    }

    private void Recieve(byte[] data)
    {
        Debug.Log("WS received message: " + Encoding.UTF8.GetString(data));
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
        PositionData data = new PositionData("recieve",transform.position);
        Channel channel = new Channel("PlayerPositionSyncChannel");

        Subscribe sb = new Subscribe("message", JsonUtility.ToJson(channel), JsonUtility.ToJson(data));
        sendUtility.Send(JsonUtility.ToJson(sb));   
    }
}
