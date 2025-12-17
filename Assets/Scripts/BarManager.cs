using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BarManager : MonoBehaviour
{
    [Header("Configurações da Barra")]
    [SerializeField] private bool useSegments = false;
    [SerializeField] private Image barImage; // Usado se useSegments = false
    [SerializeField] private GameObject barContainer; // Container para mostrar/esconder a barra
    [SerializeField] private GameObject[] barSegments; // Usado se useSegments = true
    
    private float maxDuration;
    private float remainingTime;
    private bool isActive = false;
    private int activeSegments;

    void Awake()
    {
        HideBar();
    }

    void Update()
    {
        if (isActive && remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            UpdateBar();
            
            if (remainingTime <= 0)
            {
                HideBar();
            }
        }
    }

    /// <summary>
    /// Inicia a barra com uma duração específica
    /// </summary>
    public void StartTimer(float duration)
    {
        maxDuration = duration;
        remainingTime = duration;
        isActive = true;
        
        if (useSegments && barSegments != null)
        {
            activeSegments = barSegments.Length;
            // Força ativação de todos os segmentos imediatamente
            foreach (var segment in barSegments)
            {
                if (segment != null) segment.SetActive(true);
            }
        }
        
        ShowBar();
        UpdateBar();
    }

    /// <summary>
    /// Para o timer e esconde a barra
    /// </summary>
    public void StopTimer()
    {
        isActive = false;
        remainingTime = 0;
        HideBar();
    }

    /// <summary>
    /// Reseta o timer. Opcionalmente aceita uma nova duração.
    /// </summary>
    /// <param name="newDuration">Nova duração (opcional). Se <= 0, usa a maxDuration anterior.</param>
    public void ResetTimer(float newDuration = -1f)
    {
        if (isActive)
        {
            // Se foi passada uma nova duração válida, atualiza o maxDuration
            if (newDuration > 0)
            {
                maxDuration = newDuration;
            }

            // Reseta o tempo restante para o (possivelmente novo) máximo
            remainingTime = maxDuration;
            
            // Reseta a contagem de segmentos ativos
            if (useSegments && barSegments != null)
            {
                activeSegments = barSegments.Length;
                // Força re-ativação de todos os segmentos
                foreach (var segment in barSegments)
                {
                    if (segment != null) segment.SetActive(true);
                }
            }
            
            UpdateBar();
        }
        else
        {
            // Se por algum motivo Reset for chamado com a barra inativa, inicia ela
            float durationToUse = newDuration > 0 ? newDuration : maxDuration;
            if (durationToUse > 0) StartTimer(durationToUse);
        }
    }

    private void UpdateBar()
    {
        if (useSegments && barSegments != null && barSegments.Length > 0)
        {
            UpdateSegments();
        }
        else if (barImage != null)
        {
            barImage.fillAmount = remainingTime / maxDuration;
        }
    }
    
    private void UpdateSegments()
    {
        // Calcula quantos segmentos devem estar ativos
        float percentageRemaining = remainingTime / maxDuration;
        int segmentsToShow = Mathf.CeilToInt(percentageRemaining * barSegments.Length);
        
        // Atualiza apenas se mudou o número de segmentos
        if (segmentsToShow != activeSegments)
        {
            activeSegments = segmentsToShow;
            
            // Ativa/desativa cada segmento
            for (int i = 0; i < barSegments.Length; i++)
            {
                if (barSegments[i] != null)
                {
                    barSegments[i].SetActive(i < activeSegments);
                }
            }
        }
    }

    private void ShowBar()
    {
        if (barContainer != null)
        {
            barContainer.SetActive(true);
        }
        else if (useSegments && barSegments != null)
        {
            // Ativa todos os segmentos
            foreach (var segment in barSegments)
            {
                if (segment != null) segment.SetActive(true);
            }
        }
        else if (barImage != null)
        {
            barImage.gameObject.SetActive(true);
        }
    }

    private void HideBar()
    {
        if (barContainer != null)
        {
            barContainer.SetActive(false);
        }
        else if (useSegments && barSegments != null)
        {
            // Desativa todos os segmentos
            foreach (var segment in barSegments)
            {
                if (segment != null) segment.SetActive(false);
            }
        }
        else if (barImage != null)
        {
            barImage.gameObject.SetActive(false);
        }
    }

    // Propriedade útil para verificar se está ativo
    public bool IsActive => isActive;
}