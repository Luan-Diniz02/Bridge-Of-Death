using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Collectibles : MonoBehaviour
{
    public enum CollectibleType
    {
        Coin,
        SpeedBoost,
        Shield,
        Heal
    }
    
    [Header("Configurações")]
    [SerializeField] private CollectibleType collectibleType;
    
    [Header("Power-Up Settings (se aplicável)")]
    [SerializeField] private float duration = 5f;
    [SerializeField] private float effectValue = 2f; // Multiplicador de velocidade, etc
    
    [Header("UI")]
    [SerializeField] private GameObject uiElement;
    [SerializeField] private TextMeshProUGUI infoText;
    
    [Header("Events")]
    [SerializeField] private UnityEvent onCollect;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            
            if (player != null)
            {
                ApplyEffect(player);
            }
            
            onCollect.Invoke();
            Destroy(gameObject);
        }
    }
    
    private void ApplyEffect(PlayerController player)
    {
        switch (collectibleType)
        {
            case CollectibleType.Coin:
                Debug.Log("Coletou 1 moeda!");
                break;
                
            case CollectibleType.SpeedBoost:
                // Aumenta velocidade temporariamente
                Debug.Log("Speed Boost ativado!");
                // player.ApplySpeedBoost(duration, effectValue);
                break;
                
            case CollectibleType.Shield:
                // Protege de 1 hit
                Debug.Log("Shield ativado!");
                // player.ApplyShield(duration);
                break;
                
            case CollectibleType.Heal:
                // Atrai moedas automaticamente
                Debug.Log("Heal ativado!");
                // player.ApplyHeal(duration);
                break;
        }
    }
}
