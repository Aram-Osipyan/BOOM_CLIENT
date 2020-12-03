using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public string token;
    public string id;
    public new string name;
    private void Awake()
    {
       
    }
    void Start()
    {
        instance = gameObject.GetComponent<GameManager>();
    }


    void Update()
    {
        
    }
}
