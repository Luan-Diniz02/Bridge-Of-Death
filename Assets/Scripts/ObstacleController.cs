using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ObstacleController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private AudioClip hitSound;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HideAndDestroyAfterSound();
        }
    }

    private void HideAndDestroyAfterSound()
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
