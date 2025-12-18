using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Gerencia todos os players na cena (spawn, referências, multiplayer)
/// </summary>
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    
    [Header("Spawn Configuration")]
    [SerializeField] private Transform[] spawnPoints; // Pontos de spawn para cada player
    [SerializeField] private bool spawnOnStart = true;
    
    [Header("Player Prefabs (Fallback)")]
    [SerializeField] private GameObject defaultPlayerPrefab; // Prefab padrão se não houver loja
    
    private List<GameObject> playerGameObjects = new List<GameObject>();
    private List<PlayerController> playerControllers = new List<PlayerController>();
    
    public int PlayerCount => playerControllers.Count;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    void Start()
    {
        if (spawnOnStart)
        {
            SpawnPlayers();
        }
    }
    
    /// <summary>
    /// Spawna os players baseado nas configurações
    /// </summary>
    public void SpawnPlayers()
    {
        // Limpa players existentes
        ClearPlayers();
        
        // Determina quantos players spawnar
        int playerCount = GameSettings.Instance.IsTwoPlayerMode ? 2 : 1;
        
        for (int i = 0; i < playerCount; i++)
        {
            SpawnPlayer(i);
        }
    }
    
    /// <summary>
    /// Spawna um player específico
    /// </summary>
    private void SpawnPlayer(int playerIndex)
    {
        if (playerIndex >= spawnPoints.Length)
        {
            Debug.LogError($"Spawn point {playerIndex} não existe! Adicione mais spawn points.");
            return;
        }
        
        GameObject playerPrefab = GetPlayerPrefab(playerIndex);
        if (playerPrefab == null)
        {
            Debug.LogError($"Nenhum prefab disponível para player {playerIndex}");
            return;
        }
        
        Transform spawnPoint = spawnPoints[playerIndex];
        GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        
        // Configura identificação
        player.name = $"Player{playerIndex + 1}";
        player.tag = playerIndex == 0 ? "Player" : $"Player{playerIndex + 1}";
        
        // Adiciona às listas
        playerGameObjects.Add(player);
        
        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller != null)
        {
            playerControllers.Add(controller);
            ConfigurePlayerInput(controller, playerIndex);
        }
        
        Debug.Log($"Player {playerIndex + 1} spawnado em {spawnPoint.position}");
    }
    
    /// <summary>
    /// Obtém o prefab do personagem selecionado ou padrão
    /// </summary>
    private GameObject GetPlayerPrefab(int playerIndex)
    {
        // Tenta obter da loja (personagem selecionado)
        SimpleStoreManager storeManager = FindFirstObjectByType<SimpleStoreManager>();
        if (storeManager != null)
        {
            SimpleStoreCharacter selectedCharacter = storeManager.GetSelectedCharacter();
            if (selectedCharacter != null && selectedCharacter.CharacterPrefab != null)
            {
                return selectedCharacter.CharacterPrefab;
            }
        }
        
        // Fallback para prefab padrão
        return defaultPlayerPrefab;
    }
    
    /// <summary>
    /// Configura o input do player (teclado vs gamepad)
    /// </summary>
    private void ConfigurePlayerInput(PlayerController controller, int playerIndex)
    {
        PlayerInput playerInput = controller.GetComponent<PlayerInput>();
        if (playerInput == null) return;
        
        // Player 1 = Keyboard/Mouse, Player 2 = Gamepad
        // Unity Input System gerencia isso automaticamente se configurado corretamente
        // Você pode adicionar lógica específica aqui se necessário
    }
    
    /// <summary>
    /// Obtém um player específico por índice
    /// </summary>
    public PlayerController GetPlayer(int index)
    {
        if (index < 0 || index >= playerControllers.Count)
            return null;
        return playerControllers[index];
    }
    
    /// <summary>
    /// Obtém o GameObject do player por índice
    /// </summary>
    public GameObject GetPlayerGameObject(int index)
    {
        if (index < 0 || index >= playerGameObjects.Count)
            return null;
        return playerGameObjects[index];
    }
    
    /// <summary>
    /// Obtém o player mais próximo de uma posição
    /// </summary>
    public PlayerController GetClosestPlayer(Vector3 position)
    {
        if (playerControllers.Count == 0)
            return null;
        
        PlayerController closest = null;
        float minDistance = float.MaxValue;
        
        foreach (var player in playerControllers)
        {
            if (player == null) continue;
            
            float distance = Vector3.Distance(position, player.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = player;
            }
        }
        
        return closest;
    }
    
    /// <summary>
    /// Obtém o GameObject do player mais próximo
    /// </summary>
    public GameObject GetClosestPlayerGameObject(Vector3 position)
    {
        PlayerController controller = GetClosestPlayer(position);
        return controller?.gameObject;
    }
    
    /// <summary>
    /// Obtém todos os players ativos
    /// </summary>
    public List<PlayerController> GetAllPlayers()
    {
        return new List<PlayerController>(playerControllers);
    }
    
    /// <summary>
    /// Obtém todos os GameObjects dos players
    /// </summary>
    public List<GameObject> GetAllPlayerGameObjects()
    {
        return new List<GameObject>(playerGameObjects);
    }
    
    /// <summary>
    /// Limpa todos os players da cena
    /// </summary>
    private void ClearPlayers()
    {
        foreach (var player in playerGameObjects)
        {
            if (player != null)
                Destroy(player);
        }
        
        playerGameObjects.Clear();
        playerControllers.Clear();
    }
    
    /// <summary>
    /// Remove um player específico
    /// </summary>
    public void RemovePlayer(int index)
    {
        if (index < 0 || index >= playerGameObjects.Count)
            return;
        
        if (playerGameObjects[index] != null)
            Destroy(playerGameObjects[index]);
        
        playerGameObjects.RemoveAt(index);
        
        if (index < playerControllers.Count)
            playerControllers.RemoveAt(index);
    }
    
    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
