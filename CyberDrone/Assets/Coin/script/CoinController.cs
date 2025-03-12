using System;
using UnityEngine;

public class CollectCoin : MonoBehaviour
{
    private static CollectCoin _instance;
    public static CollectCoin Instance => _instance;
    private int _score;
    public int Score =>_score;

    public event Action OnCoinCollected;

    void Awake()
    {
        if(_instance == null){
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }

    public void CollectCoins(){
        _score += 1;
        OnCoinCollected?.Invoke();
    }
}
