using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// Gerencia câmeras e atribui automaticamente aos players
/// </summary>
[DefaultExecutionOrder(-99)] // Executa logo após PlayerManager
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    
    [Header("Camera References")]
    [SerializeField] private CinemachineCamera mainVirtualCamera;
    [SerializeField] private CinemachineCamera deathCamera;
    [SerializeField] private Camera mainCamera;
    
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
        
        if (mainCamera == null)
            mainCamera = Camera.main;
    }
    
    void Start()
    {
        if (autoAssignToPlayers)
        {
            // Aguarda um frame para garantir que players foram spawnados
            Invoke(nameof(AssignCamerasToPlayers), 0.1f);
        }
    }
    
    /// <summary>
    /// Atribui câmeras aos players automaticamente
    /// </summary>
    public void AssignCamerasToPlayers()
    {
        if (PlayerManager.Instance == null)
        {
            Debug.LogWarning("PlayerManager não encontrado!");
            return;
        }
        
        // Para single player (Player 1)
        PlayerController player1 = PlayerManager.Instance.GetPlayer(0);
        if (player1 != null)
        {
            AssignCameraToPlayer(player1, mainVirtualCamera, deathCamera);
            Debug.Log("Câmera atribuída ao Player 1");
        }
        
        // TODO: Para multiplayer, criar câmeras adicionais
        // PlayerController player2 = PlayerManager.Instance.GetPlayer(1);
        // if (player2 != null) { ... }
    }
    
    /// <summary>
    /// Atribui as câmeras a um player específico
    /// </summary>
    public void AssignCameraToPlayer(PlayerController player, CinemachineCamera vcam, CinemachineCamera deathCam)
    {
        if (player == null || vcam == null)
        {
            Debug.LogWarning("Player ou câmera virtual nula!");
            return;
        }
        
        // Busca o objeto filho com a tag "LookAt"
        Transform lookAtTarget = FindLookAtTarget(player.transform);
        
        // Configura a Cinemachine Virtual Camera
        vcam.Follow = lookAtTarget != null ? lookAtTarget : player.transform; // Olha para o LookAt ou player
        
        if (deathCam != null)
        {
            deathCam.Follow = lookAtTarget != null ? lookAtTarget : player.transform;
        }
        
        // Injeta referências no CameraController do player
        CameraController playerCamController = player.GetComponent<CameraController>();
        if (playerCamController != null)
        {
            playerCamController.SetCameras(vcam, deathCam, mainCamera);
        }
    }
    
    /// <summary>
    /// Busca recursivamente o objeto filho com a tag "LookAt"
    /// </summary>
    private Transform FindLookAtTarget(Transform parent)
    {
        // Verifica todos os filhos
        foreach (Transform child in parent)
        {
            if (child.CompareTag("LookAt"))
            {
                return child;
            }
            
            // Busca recursivamente nos filhos dos filhos
            Transform found = FindLookAtTarget(child);
            if (found != null)
            {
                return found;
            }
        }
        
        return null; // Não encontrou
    }
    
    /// <summary>
    /// Obtém a câmera principal
    /// </summary>
    public Camera GetMainCamera()
    {
        return mainCamera;
    }
    
    /// <summary>
    /// Obtém a virtual camera principal
    /// </summary>
    public CinemachineCamera GetMainVirtualCamera()
    {
        return mainVirtualCamera;
    }
}
