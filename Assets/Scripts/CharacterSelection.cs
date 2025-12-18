using UnityEngine;

/// <summary>
/// Mantém dados dos personagens selecionados entre cenas (DontDestroyOnLoad)
/// Suporta 1-2 jogadores
/// </summary>
public class CharacterSelection : MonoBehaviour
{
    public static CharacterSelection Instance { get; private set; }
    
    // Player 1 (sempre presente)
    private string selectedCharacterID;
    private GameObject selectedCharacterPrefab;
    
    // Player 2 (apenas Split Screen)
    private string selectedCharacterID_Player2;
    private GameObject selectedCharacterPrefab_Player2;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Carrega seleção salva
        LoadSelection();
    }
    
    /// <summary>
    /// Define o personagem selecionado
    /// </summary>
    public void SetSelectedCharacter(string characterID, GameObject prefab)
    {
        selectedCharacterID = characterID;
        selectedCharacterPrefab = prefab;
        
        // Salva a seleção
        PlayerPrefs.SetString("SelectedCharacter", characterID);
        PlayerPrefs.Save();
        
        Debug.Log($"CharacterSelection: Personagem {characterID} salvo globalmente");
    }
    
    /// <summary>
    /// Obtém o prefab do personagem selecionado
    /// </summary>
    public GameObject GetSelectedCharacterPrefab()
    {
        return selectedCharacterPrefab;
    }
    
    /// <summary>
    /// Obtém o ID do personagem selecionado
    /// </summary>
    public string GetSelectedCharacterID()
    {
        return selectedCharacterID;
    }
    
    /// <summary>
    /// Carrega a seleção do PlayerPrefs
    /// </summary>
    private void LoadSelection()
    {
        selectedCharacterID = PlayerPrefs.GetString("SelectedCharacter", "");
        // Nota: O prefab será reconstruído pelo SimpleStoreManager ao carregar a loja
    }
    
    /// <summary>
    /// Verifica se há um personagem selecionado
    /// </summary>
    public bool HasSelection()
    {
        return !string.IsNullOrEmpty(selectedCharacterID) && selectedCharacterPrefab != null;
    }
    
    /// <summary>
    /// Reconstrói a referência do prefab (chamado pelo SimpleStoreManager)
    /// </summary>
    public void RebuildPrefabReference(GameObject prefab)
    {
        selectedCharacterPrefab = prefab;
        Debug.Log($"CharacterSelection: Prefab reconstruído para {selectedCharacterID}");
    }
    
    // ===== PLAYER 2 (SPLIT SCREEN) =====
    
    /// <summary>
    /// Define o personagem do Player 2 (Split Screen)
    /// </summary>
    public void SetPlayer2Character(string characterID, GameObject prefab)
    {
        selectedCharacterID_Player2 = characterID;
        selectedCharacterPrefab_Player2 = prefab;
        
        PlayerPrefs.SetString("SelectedCharacter_Player2", characterID);
        PlayerPrefs.Save();
        
        Debug.Log($"CharacterSelection: Player 2 - {characterID} salvo");
    }
    
    /// <summary>
    /// Obtém o prefab do Player 2
    /// </summary>
    public GameObject GetPlayer2CharacterPrefab()
    {
        return selectedCharacterPrefab_Player2;
    }
    
    /// <summary>
    /// Obtém o ID do personagem do Player 2
    /// </summary>
    public string GetPlayer2CharacterID()
    {
        return selectedCharacterID_Player2;
    }
    
    /// <summary>
    /// Verifica se o Player 2 tem personagem selecionado
    /// </summary>
    public bool HasPlayer2Selection()
    {
        return !string.IsNullOrEmpty(selectedCharacterID_Player2) && selectedCharacterPrefab_Player2 != null;
    }
    
    /// <summary>
    /// Reconstrói a referência do prefab do Player 2
    /// </summary>
    public void RebuildPlayer2PrefabReference(GameObject prefab)
    {
        selectedCharacterPrefab_Player2 = prefab;
        Debug.Log($"CharacterSelection: Player 2 prefab reconstruído para {selectedCharacterID_Player2}");
    }
    
    /// <summary>
    /// Limpa a seleção do Player 2 (quando voltar para Single Player)
    /// </summary>
    public void ClearPlayer2Selection()
    {
        selectedCharacterID_Player2 = "";
        selectedCharacterPrefab_Player2 = null;
        PlayerPrefs.DeleteKey("SelectedCharacter_Player2");
        PlayerPrefs.Save();
        Debug.Log("CharacterSelection: Player 2 limpo");
    }
}
