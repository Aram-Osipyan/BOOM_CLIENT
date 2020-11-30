using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HybridWebSocket;
using System.Text;
using SendModels;
using UnityEngine.UI;

public class SyncPosition : MonoBehaviour
{
    /// <summary>
    /// Time in seconds
    /// </summary>
    [SerializeField] float timeBetweenSend = 1f;
    [SerializeField] Text statusText;
    private float dTime;
    SendUtility sendUtility;
    void Start()
    {        
        WebSocketInit();
    }

    private void WebSocketInit()
    {
        sendUtility = new SendUtility();
        sendUtility.recieve = Recieve;
        StartCoroutine(Connect());        
    }

    IEnumerator Connect()
    {
        sendUtility.ConnectToServer();
        statusText.text = "Status: " + sendUtility.webSocket.GetState();
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            statusText.text = "Status: " + sendUtility.webSocket.GetState();            
            if (sendUtility.webSocket.GetState() == WebSocketState.Open)
            {
                break;
            }            
        }
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
        if (sendUtility.webSocket.GetState() != WebSocketState.Open)
            return;
        PositionData data = new PositionData("recieve",new PositionData.PositionVector3( transform.position));
        Channel channel = new Channel("PlayerPositionSyncChannel");

        Subscribe sb = new Subscribe("message", JsonUtility.ToJson(channel), JsonUtility.ToJson(data));
        sendUtility.Send(JsonUtility.ToJson(sb));   
    }
    private void OnDestroy()
    {
        sendUtility.webSocket.Close();
    }
}
