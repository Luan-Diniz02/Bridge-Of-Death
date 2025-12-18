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
        // Garante que CharacterSelection existe
        if (CharacterSelection.Instance == null)
        {
            GameObject selectionObj = new GameObject("CharacterSelection");
            selectionObj.AddComponent<CharacterSelection>();
        }
        
        // Encontra todos os personagens na loja
        characters = GetComponentsInChildren<SimpleStoreCharacter>(true);
        Debug.Log($"SimpleStoreManager.Start(): Encontrados {characters.Length} personagens na hierarquia de '{gameObject.name}'");
        
        // Se não encontrou, tenta buscar em toda a cena
        if (characters.Length == 0)
        {
            Debug.LogWarning("Não encontrou personagens como filhos! Buscando em toda a cena...");
            characters = FindObjectsByType<SimpleStoreCharacter>(FindObjectsSortMode.None);
            Debug.Log($"Encontrados {characters.Length} personagens na cena");
        }
        
        // Atualiza display de moedas
        UpdateCurrencyDisplay(CurrencyManager.Instance.GetCurrentCurrency());
        CurrencyManager.Instance.OnCurrencyChanged += UpdateCurrencyDisplay;
        
        // Aguarda todos os SimpleStoreCharacter.Start() executarem primeiro
        Invoke(nameof(LoadSelectedCharacter), 0.1f);
    }
    
    private void OnDestroy()
    {
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnCurrencyChanged -= UpdateCurrencyDisplay;
    }
    
    public void SelectCharacter(SimpleStoreCharacter character)
    {
        // Se o array estiver vazio, recarrega
        if (characters == null || characters.Length == 0)
        {
            Debug.LogWarning("Array de personagens estava vazio! Recarregando...");
            characters = GetComponentsInChildren<SimpleStoreCharacter>(true);
            if (characters.Length == 0)
            {
                characters = FindObjectsByType<SimpleStoreCharacter>(FindObjectsSortMode.None);
            }
        }
        
        // Desmarca todos PRIMEIRO
        foreach (var c in characters)
        {
            if (c != character)
            {
                c.SetSelected(false);
            }
        }
        
        // Marca o selecionado
        character.SetSelected(true);
        selectedCharacter = character;
        
        // Salva globalmente para usar em outras cenas
        if (CharacterSelection.Instance != null)
        {
            CharacterSelection.Instance.SetSelectedCharacter(
                character.CharacterID, 
                character.CharacterPrefab
            );
        }
        
        // Força refresh de todos para garantir
        RefreshAllCharactersUI();
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
        // IMPORTANTE: Desmarca todos primeiro para evitar múltiplas seleções
        foreach (var c in characters)
        {
            c.SetSelected(false);
        }
        
        string savedID = PlayerPrefs.GetString("SelectedCharacter", "");
        
        if (!string.IsNullOrEmpty(savedID))
        {
            var character = characters.FirstOrDefault(c => c.CharacterID == savedID);
            if (character != null && character.IsPurchased)
            {
                Debug.Log($"Encontrado personagem salvo: {savedID}");
                SelectCharacter(character);
                
                // Reconstrói a referência do prefab no CharacterSelection
                if (CharacterSelection.Instance != null)
                {
                    CharacterSelection.Instance.RebuildPrefabReference(character.CharacterPrefab);
                }
                
                RefreshAllCharactersUI();
                return;
            }
        }
        
        // Se não houver seleção salva, seleciona APENAS O PRIMEIRO personagem padrão encontrado
        var defaultCharacter = characters.FirstOrDefault(c => c.IsDefault && c.IsPurchased);
        if (defaultCharacter != null)
        {
            Debug.Log($"Selecionando personagem padrão: {defaultCharacter.CharacterID}");
            SelectCharacter(defaultCharacter);
            RefreshAllCharactersUI();
            return;
        }
        
        // Caso não encontre o padrão, seleciona o primeiro comprado
        var firstPurchased = characters.FirstOrDefault(c => c.IsPurchased);
        if (firstPurchased != null)
        {
            Debug.Log($"Selecionando primeiro personagem comprado: {firstPurchased.CharacterID}");
            SelectCharacter(firstPurchased);
            RefreshAllCharactersUI();
        }
    }
    
    /// <summary>
    /// Força atualização da UI de todos os personagens
    /// </summary>
    private void RefreshAllCharactersUI()
    {
        foreach (var character in characters)
        {
            character.RefreshUI();
        }
    }
    
    private void UpdateCurrencyDisplay(int amount)
    {
        if (currencyText != null)
            currencyText.text = amount.ToString();
    }
}
