using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [HideInInspector]
    public string token;
    [HideInInspector]
    public string id = "2";
    [HideInInspector]
    public new string name;
    [SerializeField] GameObject player;
    [SerializeField] public PlayerData playerData;
    private void Awake()
    {
       
    }
    void Start()
    {
        instance = gameObject.GetComponent<GameManager>();
    }

    public void GameStart()
    {
        //player.AddComponent<SyncPosition>();
        player.GetComponent<SyncPosition>().enabled = true;
        
    }
    void Update()
    {
        
    }
}
