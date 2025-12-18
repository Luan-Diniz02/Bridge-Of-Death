using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ObstacleController : MonoBehaviour
{
    // [SerializeField] private PlayerController playerController; // REMOVIDO - detecta automaticamente
    [SerializeField] private AudioClip hitSound;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Player2"))
        {
            // Pega o PlayerController do objeto que colidiu
            PlayerController hitPlayer = other.GetComponent<PlayerController>();
            if (hitPlayer != null)
            {
                HideAndDestroyAfterSound(hitPlayer);
            }
        }
    }

    private void HideAndDestroyAfterSound(PlayerController playerController)
    {
        playerController.DamagePlayer();
        // Desativa visual mas mantém objeto ativo para o som terminar
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
        Destroy(gameObject, hitSound.length);
    }
}
