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
    [SerializeField] float timeBetweenSend = 0.1f;
    [SerializeField] PlayerData playerData;
    
#if DEBUG_SYNC_POS
    [SerializeField] Text statusText;
#endif
    
    private float dTime;
    private SendUtility sendUtility;
    private PositionData data;
    private Channel channel = new Channel("PlayerPositionSyncChannel");
    private Subscribe sb ;
    private List<GameObject> playersInstance;
    void Start()
    {
        Debug.Log("Start");
        sb = new Subscribe("message", JsonUtility.ToJson(channel));
        data = new PositionData("recieve");
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
        var mes = JsonUtility.FromJson<RecieveModels.PositionMessage>(Encoding.UTF8.GetString(data));
        if (playersInstance == null)
        {
            playersInstance = new List<GameObject>();
            foreach (var item in mes.message.players)
            {
                if (item.position != null)
                {
                    playersInstance.Add(Instantiate(playerData.players[0]));
                }
            }
            
        }
        for (int i = 0; i < playersInstance.Count; i++)
        {
            var pos = mes.message.players[i].position;
            playersInstance[i].transform.position = new Vector3(pos.x,pos.y,pos.z);
        }
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
        data.position.SetPos(transform.position);
        sb.data = JsonUtility.ToJson(data);
        sendUtility.Send(JsonUtility.ToJson(sb));   
    }
    private void OnDestroy()
    {
        sendUtility.webSocket.Close();
    }
}
