using TMPro;
using Unity.AppUI.UI;
using UnityEngine;

public class LevelCompleted : MonoBehaviour
{
    [SerializeField] private CoinManager coinManager;
    [SerializeField] private TextMeshProUGUI coinsCollectedText, livesRemainingText, enemiesDefeatedText, totalCoinsText, titleText;
    [SerializeField] private GameObject levelCompletedPanel;
    [SerializeField] private GameObject [] OtherUI;
    [SerializeField] private MenuManager menuManager;
    private int coinsCollected;
    private int enemysDefeated;
    private int remainingLives;

    void Awake()
    {
        if(levelCompletedPanel != null) levelCompletedPanel.SetActive(false);
    }

    void Start()
    {
        // Inscreve-se no evento de morte do player
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.GetComponent<PlayerController>().onPlayerDeath.AddListener(OnPlayerDeath);
        }
    }

    void OnDestroy()
    {
        // Remove a inscrição ao destruir o objeto
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.GetComponent<PlayerController>().onPlayerDeath.RemoveListener(OnPlayerDeath);
        }
    }

    private void OnPlayerDeath()
    {
        Debug.Log("LevelCompleted notificado: Player morreu!");
        DisplayLevelFailed();
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
        if(menuManager != null) menuManager.PlayMenuMusic();
        DisableOtherUI();
        SetCoinsCollected();
        SetRemainingLives();
        enemiesDefeatedText.text = enemysDefeated.ToString();
        int totalCoins = coinsCollected + (remainingLives * 5) + (enemysDefeated * 10);
        totalCoinsText.text = totalCoins.ToString();
        
        // Adiciona as moedas ao total persistente do jogador
        CurrencyManager.Instance.AddCurrency(totalCoins);
        
        if(levelCompletedPanel != null) levelCompletedPanel.SetActive(true);
    }

    public void DisplayLevelFailed()
    {
        if(titleText != null) titleText.text = "YOU DIED";
        DisplayLevelCompletionStats();
    }
}
