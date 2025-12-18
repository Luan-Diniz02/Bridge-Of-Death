using UnityEngine;

/// <summary>
/// ScriptableObject que define os dados de um personagem comprável
/// </summary>
[CreateAssetMenu(fileName = "New Character", menuName = "Store/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Informações Básicas")]
    [SerializeField] private string characterName;
    [SerializeField] private string characterID; // ID único para salvar/carregar
    [SerializeField][TextArea(3, 5)] private string description;
    
    [Header("Visual")]
    [SerializeField] private Sprite characterIcon;
    [SerializeField] private GameObject characterPrefab;
    
    [Header("Preço")]
    [SerializeField] private int price;
    [SerializeField] private bool isDefault = false; // Personagem inicial gratuito
    
    public string CharacterName => characterName;
    public string CharacterID => characterID;
    public string Description => description;
    public Sprite CharacterIcon => characterIcon;
    public GameObject CharacterPrefab => characterPrefab;
    public int Price => price;
    public bool IsDefault => isDefault;
}
