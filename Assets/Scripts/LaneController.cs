using UnityEngine;

public class LaneController : MonoBehaviour
{
    [Header("Configurações de Faixa")]
    [SerializeField] private float laneWidth = 1.5f;
    [SerializeField] private float laneChangeSpeed = 10f;
    
    // Estado
    private int currentLane = 0; // -1: Esquerda, 0: Centro, 1: Direita
    private float centerLaneX; // A posição X original (centro)
    private bool isChangingLane = false;
    private Vector3 horizontalVelocity;

    void Awake()
    {
        // Define o ponto central baseado na posição inicial do objeto
        centerLaneX = transform.position.x;
    }

    /// <summary>
    /// Calcula a velocidade horizontal necessária para alinhar o objeto à faixa atual.
    /// Deve ser chamado no Update do controlador principal.
    /// </summary>
    public Vector3 CalculateLaneMovement(Vector3 currentPosition)
{
    if (!isChangingLane) return Vector3.zero;

    // Calcula a posição alvo no eixo X
    float targetX = centerLaneX + (currentLane * laneWidth);
    
    // Verifica a distância absoluta
    float distance = Mathf.Abs(currentPosition.x - targetX);

    // Condição de parada (Threshold)
    if (distance < 0.1f)
    {
        horizontalVelocity = Vector3.zero;
        isChangingLane = false;
        
        // Opcional: Snap para posição exata para evitar micro-deslizes
        // Mas sem alterar transform diretamente aqui
        return Vector3.zero; 
    }

    // --- CORREÇÃO AQUI ---
    // Voltamos a usar a lógica proporcional: (Destino - Posição Atual) * Velocidade
    // Isso restaura o efeito de suavização do script original.
    float moveX = (targetX - currentPosition.x) * laneChangeSpeed;
    
    horizontalVelocity = Vector3.right * moveX;

    return horizontalVelocity;
}

    /// <summary>
    /// Tenta mudar a faixa atual baseado em um incremento (-1 para esquerda, +1 para direita).
    /// </summary>
    public void MoveLane(int direction)
    {
        int newLane = currentLane + direction;
        newLane = Mathf.Clamp(newLane, -1, 1);

        if (newLane != currentLane)
        {
            currentLane = newLane;
            isChangingLane = true;
        }
    }

    // Getters úteis para animações ou lógica externa
    public bool IsChangingLane => isChangingLane;
    public int CurrentLane => currentLane;
}