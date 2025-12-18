using UnityEngine;

/// <summary>
/// Mantém dados do personagem selecionado entre cenas (DontDestroyOnLoad)
/// </summary>
public class CharacterSelection : MonoBehaviour
{
    public static CharacterSelection Instance { get; private set; }
    
    private string selectedCharacterID;
    private GameObject selectedCharacterPrefab;
    
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
    }
    
    /// <summary>
    /// Verifica se há um personagem selecionado
    /// </summary>
    public bool HasSelection()
    {
        return !string.IsNullOrEmpty(selectedCharacterID) && selectedCharacterPrefab != null;
    }
}
