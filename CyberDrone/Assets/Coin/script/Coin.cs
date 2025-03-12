using UnityEngine;

public class Coin : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Drone")){
            CollectCoin.Instance.CollectCoins();
            Destroy(gameObject);
        }
    }
}
