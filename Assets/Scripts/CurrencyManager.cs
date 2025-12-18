using UnityEngine;
using System;

/// <summary>
/// Gerencia as moedas do jogador de forma persistente
/// Single Responsibility: Apenas gerencia a moeda do jogador
/// </summary>
public class CurrencyManager : MonoBehaviour
{
    private static CurrencyManager instance;
    public static CurrencyManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("CurrencyManager");
                instance = go.AddComponent<CurrencyManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }
    
    private const string CURRENCY_KEY = "PlayerCurrency";
    private int currentCurrency;
    
    public event Action<int> OnCurrencyChanged;
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
        LoadCurrency();
    }
    
    private void OnDestroy()
    {
        if (instance == this)
        {
            // Limpa todos os listeners de eventos antes de destruir
            OnCurrencyChanged = null;
            instance = null;
        }
    }
    
    private void LoadCurrency()
    {
        currentCurrency = PlayerPrefs.GetInt(CURRENCY_KEY, 0);
        OnCurrencyChanged?.Invoke(currentCurrency);
    }
    
    public void AddCurrency(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Tentando adicionar valor negativo de moeda!");
            return;
        }
        
        currentCurrency += amount;
        SaveCurrency();
        OnCurrencyChanged?.Invoke(currentCurrency);
    }
    
    public bool SpendCurrency(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Tentando gastar valor negativo de moeda!");
            return false;
        }
        
        if (currentCurrency < amount)
        {
            Debug.Log("Moedas insuficientes!");
            return false;
        }
        
        currentCurrency -= amount;
        SaveCurrency();
        OnCurrencyChanged?.Invoke(currentCurrency);
        return true;
    }
    
    public int GetCurrentCurrency()
    {
        return currentCurrency;
    }
    
    private void SaveCurrency()
    {
        PlayerPrefs.SetInt(CURRENCY_KEY, currentCurrency);
        PlayerPrefs.Save();
    }
    
    // Método útil para debug/testes
    public void ResetCurrency()
    {
        currentCurrency = 0;
        SaveCurrency();
        OnCurrencyChanged?.Invoke(currentCurrency);
    }
}
