using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
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
    [Header("Sons")]
    [SerializeField] private AudioClip collectSound;
    private AudioSource audioSource;
    
    [Header("Events")]
    [SerializeField] private UnityEvent onCollect;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                ApplyEffect(player);
            }

            // Toca o som
            if (audioSource != null && collectSound != null)
            {
                audioSource.PlayOneShot(collectSound);
            }

            onCollect.Invoke();

            HideAndDestroyAfterSound();
        }
    }

    private void HideAndDestroyAfterSound()
    {
        // Desativa visual mas mantém objeto ativo para o som terminar
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        // Destroi após o som terminar
        float soundLength = collectSound != null ? collectSound.length : 0f;
        Destroy(gameObject, soundLength);
    }

    private void ApplyEffect(PlayerController player)
    {
        switch (collectibleType)
        {
            case CollectibleType.Coin:
                Debug.Log("Coletou 1 moeda!");
                break;
                
            case CollectibleType.SpeedBoost:
                Debug.Log("Speed Boost ativado!");
                player.ApplySpeedBoost(duration, effectValue);
                break;
                
            case CollectibleType.Shield:
                Debug.Log("Shield ativado!");
                player.ApplyShield(duration);
                break;
                
            case CollectibleType.Heal:
                Debug.Log("Heal ativado!");
                break;
        }
    }
}
