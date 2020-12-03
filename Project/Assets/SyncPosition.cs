//#define DEBUG_SYNC_POS
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
#if DEBUG_SYNC_POS
    [SerializeField] Text statusText;
#endif
    
    private float dTime;
    SendUtility sendUtility;
    void Start()
    {        
        
    }

    private void WebSocketInit()
    {
        sendUtility = new SendUtility();
        sendUtility.recieve = Recieve;
        StartCoroutine(Connect());        
    }

    IEnumerator Connect()
    {


        sendUtility.ConnectToServer($"ws://localhost:3000/cable?token={GameManager.instance.token}");
#if DEBUG_SYNC_POS
        statusText.text = "Status: " + sendUtility.webSocket.GetState();
#endif
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
#if DEBUG_SYNC_POS
            statusText.text = "Status: " + sendUtility.webSocket.GetState();
#endif
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
