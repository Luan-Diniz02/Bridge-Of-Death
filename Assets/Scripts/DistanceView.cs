using System;
using TMPro;
using UnityEngine;

public class DistanceView : MonoBehaviour
{
    // [SerializeField] private Transform playerTransform; // REMOVIDO - pega automaticamente
    [SerializeField] private int playerIndex = 0; // Qual player seguir (0 = Player 1, 1 = Player 2)
    [SerializeField] private TextMeshProUGUI distanceText;
    
    private Transform playerTransform;
    private float startZPosition;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (distanceText == null)
        {
            Debug.LogError("DistanceView: Distance Text is not assigned.");
            return;
        }
        
        // Busca o player através do PlayerManager
        FindPlayer();
    }
    
    private void FindPlayer()
    {
        if (PlayerManager.Instance == null)
        {
            Debug.LogWarning("PlayerManager não encontrado!");
            return;
        }
        
        PlayerController player = PlayerManager.Instance.GetPlayer(playerIndex);
        if (player != null)
        {
            playerTransform = player.transform;
            startZPosition = playerTransform.position.z;
        }
        else
        {
            Debug.LogWarning($"Player {playerIndex} não encontrado!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Se não tem referência, tenta encontrar
        if (playerTransform == null)
        {
            FindPlayer();
            return;
        }
        
        float distanceTravelled = playerTransform.position.z - startZPosition;
        distanceTravelled = Mathf.Abs(distanceTravelled);
        distanceText.text = $"Distance: {distanceTravelled:F2} m";
    }


}
