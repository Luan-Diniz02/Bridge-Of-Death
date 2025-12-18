using UnityEngine;
using TMPro;
using System.Linq;

/// <summary>
/// Gerenciador simples da loja
/// </summary>
public class SimpleStoreManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI currencyText;
    
    private SimpleStoreCharacter[] characters;
    private SimpleStoreCharacter selectedCharacter;

    private void Start()
    {
        //PlayerPrefs.DeleteAll(); // REMOVER APÓS TESTES
        // Encontra todos os personagens na loja
        characters = GetComponentsInChildren<SimpleStoreCharacter>(true);
        
        // Atualiza display de moedas
        UpdateCurrencyDisplay(CurrencyManager.Instance.GetCurrentCurrency());
        CurrencyManager.Instance.OnCurrencyChanged += UpdateCurrencyDisplay;
        
        // Carrega personagem selecionado
        LoadSelectedCharacter();
    }
    
    private void OnDestroy()
    {
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnCurrencyChanged -= UpdateCurrencyDisplay;
    }
    
    public void SelectCharacter(SimpleStoreCharacter character)
    {
        // Desmarca todos
        foreach (var c in characters)
        {
            c.SetSelected(false);
        }
        
        // Marca o selecionado
        character.SetSelected(true);
        selectedCharacter = character;
        
        Debug.Log($"Personagem {character.CharacterID} selecionado!");
    }
    
    public SimpleStoreCharacter GetSelectedCharacter()
    {
        return selectedCharacter;
    }
    
    public string GetSelectedCharacterID()
    {
        return selectedCharacter?.CharacterID;
    }
    
    private void LoadSelectedCharacter()
    {
        string savedID = PlayerPrefs.GetString("SelectedCharacter", "");
        
        if (!string.IsNullOrEmpty(savedID))
        {
            var character = characters.FirstOrDefault(c => c.CharacterID == savedID);
            if (character != null && character.IsPurchased)
            {
                character.SetSelected(true);
                selectedCharacter = character;
                return;
            }
        }
        
        // Se não houver seleção salva, seleciona o personagem padrão (IsDefault = true)
        var defaultCharacter = characters.FirstOrDefault(c => c.IsDefault && c.IsPurchased);
        if (defaultCharacter != null)
        {
            defaultCharacter.SetSelected(true);
            selectedCharacter = defaultCharacter;
            return;
        }
        
        // Caso não encontre o padrão, seleciona o primeiro comprado
        var firstPurchased = characters.FirstOrDefault(c => c.IsPurchased);
        if (firstPurchased != null)
        {
            firstPurchased.SetSelected(true);
            selectedCharacter = firstPurchased;
        }
    }
    
    private void UpdateCurrencyDisplay(int amount)
    {
        if (currencyText != null)
            currencyText.text = amount.ToString();
    }
}
