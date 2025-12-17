using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BarManager : MonoBehaviour
{
    [Header("Configurações da Barra")]
    [SerializeField] private Image barImage;
    [SerializeField] private float maxValue = 100f;
    private float currentValue;

    void Awake()
    {
        currentValue = maxValue;
        UpdateBar();
    }

    void Update()
    {
        UpdateBar();
    }

    public void SetValue(float value)
    {
        currentValue = Mathf.Clamp(value, 0, maxValue);
        UpdateBar();
    }

    private void UpdateBar()
    {
        if (barImage != null)
        {
            barImage.fillAmount = currentValue / maxValue;
        }
    }
}
