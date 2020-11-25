using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
// Use plugin namespace
using HybridWebSocket;
using System.Net.Sockets;
using System;
using UnityEngine.UI;
using System.Text.Json;
public class WebSocketDemo : MonoBehaviour {

    // Use this for initialization
    WebSocket ws;
    
    void Start () 
    {
        CreateConnection();
        //StartCoroutine(EverySec(20));
        
    }
	void CreateConnection()
    {
        // Create WebSocket instance
        ws = WebSocketFactory.CreateInstance("ws://localhost:3000/cable");
        //WebSocketFactory.CreateInstance("").
        // Add OnOpen event listener
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
        
    }
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //ws.Send(Encoding.UTF8.GetBytes("{ id: \"ChatChannel\", room: \"Best Room\" }"));
            string text = "{\"command\" : \"subscribe\",\"identifier\" : \"{\"channel\" : \"ChatChannel\" }\"}";
            ws.Send(GetComponent<Text>().text);
            
        }
	}
    IEnumerator EverySec(int secs)
    {
        for (int i = 0; i < secs; i++)
        {
            //ws.Send(Encoding.UTF8.GetBytes("Hello from Unity 3D!"));
            Debug.Log("send");
            yield return new WaitForSeconds(1);
        }
    }
    private void OnDestroy()
    {
        ws.Close();
    }
}
