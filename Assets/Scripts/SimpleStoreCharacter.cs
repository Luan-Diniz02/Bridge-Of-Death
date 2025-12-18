using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Script simples para anexar a cada personagem na UI da loja
/// </summary>
public class SimpleStoreCharacter : MonoBehaviour
{
    [Header("Identificação")]
    [SerializeField] private string characterID; // ID único (ex: "character_1")
    [SerializeField] private int price = 50;
    [SerializeField] private bool isDefault = false; // Personagem inicial gratuito
    [SerializeField] private GameObject characterPrefab; // Prefab que será instanciado no jogo
    
    [Header("UI References")]
    [SerializeField] private Button actionButton; // Botão único para comprar/selecionar
    [SerializeField] private TextMeshProUGUI buttonText; // Texto do botão
    [SerializeField] private GameObject coinIcon; // Ícone da moeda (mostrado só quando não comprado)
    [SerializeField] private GameObject lockedOverlay; // Opcional: overlay quando bloqueado
    [SerializeField] private GameObject selectedIndicator; // Opcional: indicador de selecionado
    
    private bool isPurchased;
    private bool isSelected;
    
    public string CharacterID => characterID;
    public bool IsPurchased => isPurchased;
    public bool IsSelected => isSelected;
    public GameObject CharacterPrefab => characterPrefab;
    
    private void Start()
    {
        // Configura o botão
        if (actionButton != null)
            actionButton.onClick.AddListener(OnButtonClicked);
        
        // Carrega estado salvo
        LoadState();
        UpdateUI();
    }
    
    private void OnButtonClicked()
    {
        if (!isPurchased)
        {
            TryBuy();
        }
        else if (!isSelected)
        {
            Select();
        }
    }
    
    private void TryBuy()
    {
        if (CurrencyManager.Instance.GetCurrentCurrency() < price)
        {
            Debug.Log("Moedas insuficientes!");
            return;
        }
        
        if (CurrencyManager.Instance.SpendCurrency(price))
        {
            isPurchased = true;
            SaveState();
            UpdateUI();
            
            // Automaticamente seleciona após comprar
            Select();
            
            Debug.Log($"Personagem {characterID} comprado!");
        }
    }
    
    private void Select()
    {
        // Notifica o gerenciador para desmarcar outros
        SimpleStoreManager manager = FindFirstObjectByType<SimpleStoreManager>();
        if (manager != null)
        {
            manager.SelectCharacter(this);
        }
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        
        if (isSelected)
        {
            SaveSelection();
        }
        
        UpdateUI();
    }
    
    private void UpdateUI()
    {
        if (actionButton == null)
            return;
        
        bool canAfford = CurrencyManager.Instance.GetCurrentCurrency() >= price;
        
        if (!isPurchased)
        {
            // Estado: Não comprado - Mostra preço
            if (buttonText != null)
                buttonText.text = isDefault ? "GRÁTIS" : price.ToString();
            
            if (coinIcon != null)
                coinIcon.SetActive(!isDefault); // Mostra ícone da moeda só se não for gratuito
            
            // Botão sempre clicável, valida moedas ao clicar
            actionButton.interactable = true;
        }
        else if (!isSelected)
        {
            // Estado: Comprado mas não selecionado - Mostra "SELECIONAR"
            if (buttonText != null)
                buttonText.text = "SELECIONAR";
            
            if (coinIcon != null)
                coinIcon.SetActive(false);
            
            actionButton.interactable = true;
        }
        else
        {
            // Estado: Selecionado - Mostra "SELECIONADO"
            if (buttonText != null)
                buttonText.text = "SELECIONADO";
            
            if (coinIcon != null)
                coinIcon.SetActive(false);
            
            actionButton.interactable = false;
        }
        
        // Atualiza overlays opcionais
        if (lockedOverlay != null)
            lockedOverlay.SetActive(!isPurchased);
            
        if (selectedIndicator != null)
            selectedIndicator.SetActive(isSelected);
    }
    
    private void LoadState()
    {
        // Carrega se foi comprado
        string purchaseKey = $"Character_Purchased_{characterID}";
        isPurchased = PlayerPrefs.GetInt(purchaseKey, isDefault ? 1 : 0) == 1;
    }
    
    private void SaveState()
    {
        string purchaseKey = $"Character_Purchased_{characterID}";
        PlayerPrefs.SetInt(purchaseKey, isPurchased ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    private void SaveSelection()
    {
        PlayerPrefs.SetString("SelectedCharacter", characterID);
        PlayerPrefs.Save();
    }
    
    // Para atualizar UI quando moedas mudarem
    private void OnEnable()
    {
        CurrencyManager.Instance.OnCurrencyChanged += OnCurrencyChanged;
    }
    
    private void OnDisable()
    {
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnCurrencyChanged -= OnCurrencyChanged;
    }
    
    private void OnCurrencyChanged(int amount)
    {
        UpdateUI();
    }
}
