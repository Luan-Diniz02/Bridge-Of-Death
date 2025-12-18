using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// Gerencia a loja do jogo
/// Single Responsibility: Apenas gerencia a lógica da loja
/// Dependency Inversion: Depende de abstrações (CharacterData) não de implementações
/// </summary>
public class StoreManager : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private Transform storeItemsContainer;
    [SerializeField] private GameObject storeItemPrefab;
    
    [Header("Personagens Disponíveis")]
    [SerializeField] private List<CharacterData> availableCharacters = new List<CharacterData>();
    
    private List<StoreItem> storeItems = new List<StoreItem>();
    private StoreItem selectedCharacter;
    
    private const string SELECTED_CHARACTER_KEY = "SelectedCharacter";
    
    public event Action<CharacterData> OnCharacterSelected;
    public event Action<StoreItem> OnItemPurchased;
    
    private void Start()
    {
        InitializeStore();
        UpdateCurrencyDisplay(CurrencyManager.Instance.GetCurrentCurrency());
        
        // Inscreve-se no evento de mudança de moeda
        CurrencyManager.Instance.OnCurrencyChanged += UpdateCurrencyDisplay;
    }
    
    private void OnDestroy()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCurrencyChanged -= UpdateCurrencyDisplay;
        }
    }
    
    private void InitializeStore()
    {
        // Cria itens da loja baseados nos CharacterData
        foreach (var characterData in availableCharacters)
        {
            StoreItem item = new StoreItem(characterData);
            storeItems.Add(item);
            
            // Carrega estado de compra
            LoadItemPurchaseState(item);
        }
        
        // Carrega o personagem selecionado
        LoadSelectedCharacter();
        
        // Cria UI para cada item
        CreateStoreUI();
    }
    
    private void CreateStoreUI()
    {
        // Limpa itens existentes
        foreach (Transform child in storeItemsContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Cria novo UI para cada item
        foreach (var item in storeItems)
        {
            GameObject itemUI = Instantiate(storeItemPrefab, storeItemsContainer);
            StoreItemUI itemUIComponent = itemUI.GetComponent<StoreItemUI>();
            
            if (itemUIComponent != null)
            {
                itemUIComponent.Setup(item, this);
            }
        }
    }
    
    public bool TryPurchaseItem(StoreItem item)
    {
        if (item.IsPurchased)
        {
            Debug.Log("Item já foi comprado!");
            return false;
        }
        
        int currentCurrency = CurrencyManager.Instance.GetCurrentCurrency();
        
        if (!item.CanPurchase(currentCurrency))
        {
            Debug.Log("Moedas insuficientes para comprar este item!");
            return false;
        }
        
        // Tenta gastar a moeda
        if (CurrencyManager.Instance.SpendCurrency(item.CharacterData.Price))
        {
            item.Purchase();
            SaveItemPurchaseState(item);
            
            OnItemPurchased?.Invoke(item);
            
            Debug.Log($"Personagem {item.CharacterData.CharacterName} comprado com sucesso!");
            return true;
        }
        
        return false;
    }
    
    public void SelectCharacter(StoreItem item)
    {
        if (!item.IsPurchased)
        {
            Debug.Log("Você precisa comprar este personagem primeiro!");
            return;
        }
        
        // Desmarca todos os personagens
        foreach (var storeItem in storeItems)
        {
            storeItem.IsSelected = false;
        }
        
        // Marca o personagem selecionado
        item.IsSelected = true;
        selectedCharacter = item;
        
        // Salva a seleção
        SaveSelectedCharacter(item);
        
        OnCharacterSelected?.Invoke(item.CharacterData);
        
        Debug.Log($"Personagem {item.CharacterData.CharacterName} selecionado!");
        
        // Atualiza a UI
        RefreshStoreUI();
    }
    
    public StoreItem GetSelectedCharacter()
    {
        return selectedCharacter;
    }
    
    public CharacterData GetSelectedCharacterData()
    {
        return selectedCharacter?.CharacterData;
    }
    
    private void UpdateCurrencyDisplay(int amount)
    {
        if (currencyText != null)
        {
            currencyText.text = amount.ToString();
        }
    }
    
    private void RefreshStoreUI()
    {
        foreach (Transform child in storeItemsContainer)
        {
            StoreItemUI itemUI = child.GetComponent<StoreItemUI>();
            if (itemUI != null)
            {
                itemUI.RefreshUI();
            }
        }
    }
    
    #region Save/Load
    
    private void SaveItemPurchaseState(StoreItem item)
    {
        string key = $"Character_Purchased_{item.CharacterData.CharacterID}";
        PlayerPrefs.SetInt(key, item.IsPurchased ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    private void LoadItemPurchaseState(StoreItem item)
    {
        string key = $"Character_Purchased_{item.CharacterData.CharacterID}";
        item.IsPurchased = PlayerPrefs.GetInt(key, item.CharacterData.IsDefault ? 1 : 0) == 1;
    }
    
    private void SaveSelectedCharacter(StoreItem item)
    {
        PlayerPrefs.SetString(SELECTED_CHARACTER_KEY, item.CharacterData.CharacterID);
        PlayerPrefs.Save();
    }
    
    private void LoadSelectedCharacter()
    {
        string selectedID = PlayerPrefs.GetString(SELECTED_CHARACTER_KEY, "");
        
        // Se não houver seleção salva, seleciona o primeiro personagem comprado
        if (string.IsNullOrEmpty(selectedID))
        {
            var firstPurchased = storeItems.FirstOrDefault(item => item.IsPurchased);
            if (firstPurchased != null)
            {
                firstPurchased.IsSelected = true;
                selectedCharacter = firstPurchased;
                SaveSelectedCharacter(firstPurchased);
            }
        }
        else
        {
            var selected = storeItems.FirstOrDefault(item => item.CharacterData.CharacterID == selectedID);
            if (selected != null && selected.IsPurchased)
            {
                selected.IsSelected = true;
                selectedCharacter = selected;
            }
        }
    }
    
    #endregion
    
    #region Debug Methods
    
    [ContextMenu("Unlock All Characters")]
    private void UnlockAllCharacters()
    {
        foreach (var item in storeItems)
        {
            item.IsPurchased = true;
            SaveItemPurchaseState(item);
        }
        RefreshStoreUI();
        Debug.Log("Todos os personagens foram desbloqueados!");
    }
    
    [ContextMenu("Reset All Purchases")]
    private void ResetAllPurchases()
    {
        foreach (var item in storeItems)
        {
            item.IsPurchased = item.CharacterData.IsDefault;
            SaveItemPurchaseState(item);
        }
        RefreshStoreUI();
        Debug.Log("Todas as compras foram resetadas!");
    }
    
    #endregion
}
