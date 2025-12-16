using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsText;
    private int coinCount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        coinsText.text = "Coins: 0";
    }

    public void UpdateCoinCount(int amount)
    {
        coinCount += amount;
        coinsText.text = "Coins: " + coinCount.ToString();
    }
}
