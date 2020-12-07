using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Players data", menuName = "Players data", order = 51)]
public class PlayerData : ScriptableObject
{
    [SerializeField] public GameObject[] players;

    [SerializeField] public GameObject[] bombs;
}
