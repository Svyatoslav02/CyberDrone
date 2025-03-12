using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    [SerializeField]private Text _scoreText;
    private void OnEnable()
    {
        CollectCoin.Instance.OnCoinCollected += UpdateScore;
    }

    private void OnDisable()
    {
        CollectCoin.Instance.OnCoinCollected -= UpdateScore;
    }

    private void UpdateScore(){
        _scoreText.text = "Score: " + CollectCoin.Instance.Score;
    }
}
