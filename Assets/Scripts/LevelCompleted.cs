using TMPro;
using UnityEngine;

public class LevelCompleted : MonoBehaviour
{
    [SerializeField] private CoinManager coinManager;
    [SerializeField] private TextMeshProUGUI coinsCollectedText, livesRemainingText, enemiesDefeatedText, totalCoinsText;
    [SerializeField] private GameObject levelCompletedPanel;
    [SerializeField] private GameObject [] OtherUI;
    private int coinsCollected;
    private int enemysDefeated;
    private int remainingLives;

    void Awake()
    {
        if(levelCompletedPanel != null) levelCompletedPanel.SetActive(false);
    }

    public void IncrementEnemiesDefeated(int count)
    {
        enemysDefeated += count;
    }

    private void SetRemainingLives()
    {
        remainingLives = (int)PlayerStats.Instance.Health;
        livesRemainingText.text = remainingLives.ToString();
    }

    private void SetCoinsCollected()
    {
        coinsCollected = coinManager.GetTotalCoins();
        coinsCollectedText.text = coinsCollected.ToString();
    }

    private void DisableOtherUI()
    {
        foreach(GameObject ui in OtherUI)
        {
            if(ui != null) ui.SetActive(false);
        }
    }

    public void DisplayLevelCompletionStats()
    {
        Time.timeScale = 0;
        DisableOtherUI();
        SetCoinsCollected();
        SetRemainingLives();
        enemiesDefeatedText.text = enemysDefeated.ToString();
        int totalCoins = coinsCollected + (remainingLives * 5) + (enemysDefeated * 10);
        totalCoinsText.text = totalCoins.ToString();
        if(levelCompletedPanel != null) levelCompletedPanel.SetActive(true);
    }
}
