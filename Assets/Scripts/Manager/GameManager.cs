using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<Player> players = new List<Player>();
    
    private HandManager handManager;
    [SerializeField] private HandManager handManagerPrefab;
    
    void Start()
    {
        SetupHand();
    }

    void SetupHand()
    {
        handManager = Instantiate(handManagerPrefab);
        handManager.Init(players, false); // todo
    }
}