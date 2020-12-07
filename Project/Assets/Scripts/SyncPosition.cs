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
    /// </summary>
    [SerializeField] float timeBetweenSend = 1f;
    public int playersCount = 5; 
    
#if DEBUG_SYNC_POS
    [SerializeField] Text statusText;
#endif
    
    private float dTime;
    private SendUtility sendUtility;
    private PositionData data;
    private Channel channel = new Channel("PlayerPositionSyncChannel");
    private Subscribe sb ;
    private Dictionary<string, GameObject> playersInstance = new Dictionary<string, GameObject>();
    private List<GameObject> playersPool;
    private int playerPoiner = 0;
    void Start()
    {
        Debug.Log("Start");
        sb = new Subscribe("message", JsonUtility.ToJson(channel));
        data = new PositionData("recieve");
        WebSocketInit();
        PlayersInstatiate();
    }

    private void PlayersInstatiate()
    {
        playersPool = new List<GameObject>(playersCount);
        for(int i = 0; i<playersCount; ++i)
        {
            playersPool[i] = Instantiate(GameManager.instance.playerData.players[0]);
        }
    }
    private GameObject PlayerInstantiate()
    {
        if (playerPoiner>6)
        {
            throw new Exception("error in player instantiate");
        }
        return playersPool[playerPoiner++];
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
        if (!jsonData.Contains("ping"))
        {
            var mes = JsonUtility.FromJson<RecieveModels.PositionMessage>(jsonData);
            foreach (var item in mes.message)
            {
                var playerInfo = JsonUtility.FromJson<RecieveModels.PositionMessage.Player>(item);
                if (!playersInstance.ContainsKey(playerInfo.id))
                {
                    
                    playersInstance[playerInfo.id] = PlayerInstantiate();
                    
                    
                    //pl.GetComponent<Player>().id = playerInfo.id;
                }
                var pos = playerInfo.position;
                playersInstance[playerInfo.id].transform.position = new Vector3(pos.x,pos.y,pos.z);
            }
            
        }
        //var ser =  JsonSerializer.Deserialize(jsonData,typeof(RecieveModels.PositionMessage)) as RecieveModels.PositionMessage;
       // var mes = JsonUtility.FromJson<RecieveModels.PositionMessage>(jsonData);   

        //Debug.Log();
        //var s_mes = Encoding.UTF8.GetString(data);
        //Debug.Assert(mes != null, " WS received message: " + Encoding.UTF8.GetString(data));
        //Debug.Log(playersInstance?.Count);
#if DEBUG_SERVER_CONNECT
       
        
#endif  
        /*
        for (int i = 0; i < playersInstance.Count; i++)
        {
            var pos = JsonUtility.FromJson<RecieveModels.PositionMessage.Player.PositionVector3>(mes.message.players[i].position);
            playersInstance[i].transform.position = new Vector3(pos.x,pos.y,pos.z);
        }
        */
        Debug.Log("WS : " + Encoding.UTF8.GetString(data));
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
