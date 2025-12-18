using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controla a UI de um item individual na loja
/// Single Responsibility: Apenas gerencia a visualização do item
/// </summary>
public class StoreItemUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image characterIcon;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private Button selectButton;
    [SerializeField] private GameObject lockedOverlay;
    [SerializeField] private GameObject selectedIndicator;
    
    [Header("Button Labels")]
    [SerializeField] private TextMeshProUGUI purchaseButtonText;
    [SerializeField] private TextMeshProUGUI selectButtonText;
    
    private StoreItem storeItem;
    private StoreManager storeManager;
    
    public void Setup(StoreItem item, StoreManager manager)
    {
        storeItem = item;
        storeManager = manager;
        
        // Configura os listeners dos botões
        if (purchaseButton != null)
        {
            purchaseButton.onClick.RemoveAllListeners();
            purchaseButton.onClick.AddListener(OnPurchaseClicked);
        }
        
        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnSelectClicked);
        }
        
        // Atualiza a UI inicial
        RefreshUI();
    }
    
    public void RefreshUI()
    {
        if (storeItem == null || storeItem.CharacterData == null)
            return;
        
        // Atualiza ícone e nome
        if (characterIcon != null)
            characterIcon.sprite = storeItem.CharacterData.CharacterIcon;
            
        if (characterNameText != null)
            characterNameText.text = storeItem.CharacterData.CharacterName;
        
        // Atualiza preço
        if (priceText != null)
        {
            if (storeItem.CharacterData.IsDefault)
            {
                priceText.text = "GRÁTIS";
            }
            else
            {
                priceText.text = $"{storeItem.CharacterData.Price}";
            }
        }
        
        // Atualiza estado dos botões
        UpdateButtonStates();
        
        // Atualiza overlay de bloqueio
        if (lockedOverlay != null)
            lockedOverlay.SetActive(!storeItem.IsPurchased);
        
        // Atualiza indicador de seleção
        if (selectedIndicator != null)
            selectedIndicator.SetActive(storeItem.IsSelected);
    }
    
    private void UpdateButtonStates()
    {
        bool isPurchased = storeItem.IsPurchased;
        bool isSelected = storeItem.IsSelected;
        int currentCurrency = CurrencyManager.Instance.GetCurrentCurrency();
        bool canAfford = currentCurrency >= storeItem.CharacterData.Price;
        
        // Botão de compra
        if (purchaseButton != null)
        {
            purchaseButton.gameObject.SetActive(!isPurchased);
            purchaseButton.interactable = canAfford;
            
            if (purchaseButtonText != null)
            {
                if (canAfford)
                {
                    purchaseButtonText.text = "COMPRAR";
                }
                else
                {
                    purchaseButtonText.text = "INSUFICIENTE";
                }
            }
        }
        
        // Botão de seleção
        if (selectButton != null)
        {
            selectButton.gameObject.SetActive(isPurchased);
            selectButton.interactable = !isSelected;
            
            if (selectButtonText != null)
            {
                if (isSelected)
                {
                    selectButtonText.text = "SELECIONADO";
                }
                else
                {
                    selectButtonText.text = "SELECIONAR";
                }
            }
        }
    }
    
    private void OnPurchaseClicked()
    {
        if (storeManager != null)
        {
            bool success = storeManager.TryPurchaseItem(storeItem);
            if (success)
            {
                RefreshUI();
                // Automaticamente seleciona o personagem após a compra
                storeManager.SelectCharacter(storeItem);
            }
        }
    }
    
    private void OnSelectClicked()
    {
        if (storeManager != null)
        {
            storeManager.SelectCharacter(storeItem);
        }
    }
}
