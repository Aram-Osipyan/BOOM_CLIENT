//#define DEBUG_SYNC_POS
#define DEBUG_SERVER_CONNECT
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
    /// change value for more efficienty
    /// </summary>
    [SerializeField] float timeBetweenSend = 0.1f;
    public int playersCount = 4;
    [SerializeField] List<Transform> positions; 
#if DEBUG_SYNC_POS
    [SerializeField] Text statusText;
#endif
    
    private float dTime;
    private SendUtility sendUtility;
    private PositionData data;
    private Channel channel = new Channel("PlayerPositionSyncChannel");
    private Subscribe sb ;
    private Dictionary<string, GameObject> playersInstance = new Dictionary<string, GameObject>();
    private Queue<RecieveModels.PositionMessage.Player> playerProcQueue 
        = new Queue<RecieveModels.PositionMessage.Player>();

    void Start()
    {
        Debug.Log("Start");
        sb = new Subscribe("message", JsonUtility.ToJson(channel));
        data = new PositionData("recieve");
        WebSocketInit();
        SelfInit();
    }

    private void SelfInit()
    {
        transform.position = positions[int.Parse(GameManager.instance.id) % playersCount].position;
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
        var jsonData = Encoding.UTF8.GetString(data);

        if (jsonData.Contains("ping")) // dont process ping messages
        {
            return;
        }
        
        var mes = JsonUtility.FromJson<RecieveModels.PositionMessage>(jsonData);
        foreach (var item in mes.message)
        {
            var playerInfo = JsonUtility.FromJson<RecieveModels.PositionMessage.Player>(item);
            //AddWorkInQueue(playerInfo);
            playerProcQueue.Enqueue(playerInfo);
        }            
        
        Debug.Log("WS : " + Encoding.UTF8.GetString(data));
    }

    private void ProcessPlayer()
    {
        if (playerProcQueue.Count == 0)
        {
            return;
        }
        var playerInfo = playerProcQueue.Dequeue();
        if (playerInfo.id == GameManager.instance.id)
        {
            return;
        }
        if (!playersInstance.ContainsKey(playerInfo.id))
        {
            int id = int.Parse(playerInfo.id);
            playersInstance[playerInfo.id] = Instantiate
                (
                    GameManager.instance.playerData.players[0],
                    positions[id].position,Quaternion.identity
                );
        }
        var pos = playerInfo.position;
        playersInstance[playerInfo.id].transform.position = new Vector3(pos.x, pos.y, pos.z);
        
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
        ProcessPlayer();
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
