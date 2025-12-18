using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;
    private int coinCount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        coinsText.text = "0";
    }

    public void UpdateCoinCount(int amount)
    {
        coinCount += amount;
        coinsText.text = coinCount.ToString();
    }

    public int GetTotalCoins()
    {
        return coinCount;
    }
}
