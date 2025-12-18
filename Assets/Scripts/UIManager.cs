using UnityEngine;

/// <summary>
/// Gerencia elementos de UI e atribui automaticamente aos players
/// </summary>
[DefaultExecutionOrder(-98)] // Executa após PlayerManager e CameraManager
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [Header("Player 1 UI References")]
    [SerializeField] private BarManager player1SpeedBoostBar;
    [SerializeField] private BarManager player1ShieldBar;
    
    [Header("Player 2 UI References (Multiplayer)")]
    [SerializeField] private BarManager player2SpeedBoostBar;
    [SerializeField] private BarManager player2ShieldBar;
    
    [Header("Settings")]
    [SerializeField] private bool autoAssignToPlayers = true;
    
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
        if (autoAssignToPlayers)
        {
            // Aguarda um frame para garantir que players foram spawnados
            Invoke(nameof(AssignUIToPlayers), 0.15f);
        }
    }
    
    /// <summary>
    /// Atribui elementos de UI aos players automaticamente
    /// </summary>
    public void AssignUIToPlayers()
    {
        if (PlayerManager.Instance == null)
        {
            Debug.LogWarning("PlayerManager não encontrado!");
            return;
        }
        
        // Player 1
        PlayerController player1 = PlayerManager.Instance.GetPlayer(0);
        if (player1 != null)
        {
            AssignUIToPlayer(player1, player1SpeedBoostBar, player1ShieldBar);
            Debug.Log("UI atribuída ao Player 1");
        }
        
        // Player 2 (se existir)
        if (GameSettings.Instance.IsTwoPlayerMode)
        {
            PlayerController player2 = PlayerManager.Instance.GetPlayer(1);
            if (player2 != null)
            {
                AssignUIToPlayer(player2, player2SpeedBoostBar, player2ShieldBar);
                Debug.Log("UI atribuída ao Player 2");
            }
        }
    }
    
    /// <summary>
    /// Atribui barras de UI a um player específico
    /// </summary>
    public void AssignUIToPlayer(PlayerController player, BarManager speedBoostBar, BarManager shieldBar)
    {
        if (player == null)
        {
            Debug.LogWarning("Player nulo!");
            return;
        }
        
        // Injeta as referências de UI no PlayerController
        player.SetUIBars(speedBoostBar, shieldBar);
    }
    
    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
