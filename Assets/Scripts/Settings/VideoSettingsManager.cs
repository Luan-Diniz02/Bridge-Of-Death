using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Single Responsibility: Gerencia apenas configurações de vídeo
/// Open/Closed: Pode ser estendido sem modificar o código
/// </summary>
public class VideoSettingsManager : MonoBehaviour, ISettingsManager
{
    [SerializeField] private Button fullscreenButton;
    [SerializeField] private Button fpsButton;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TextMeshProUGUI fullscreenButtonText;
    [SerializeField] private TextMeshProUGUI fpsButtonText;
    [SerializeField] private TextMeshProUGUI fpsText;
    
    private SettingsDataManager dataManager;
    private Resolution[] resolutions;
    private bool isFullscreen = true;
    private bool showFPS = false;
    private float deltaTime = 0.0f;
    
    public void SetDataManager(SettingsDataManager manager)
    {
        dataManager = manager;
    }
    
    public void Initialize()
    {
        InitializeResolutions();
        
        if (fullscreenButton != null)
            fullscreenButton.onClick.AddListener(ToggleFullscreen);
            
        if (fpsButton != null)
            fpsButton.onClick.AddListener(ToggleFPS);
            
        if (resolutionDropdown != null)
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }
    
    public void LoadSettings()
    {
        if (dataManager == null) return;
        
        isFullscreen = dataManager.GetFullscreen();
        Screen.fullScreen = isFullscreen;
        UpdateFullscreenButtonText();
        
        int resolutionIndex = dataManager.GetResolutionIndex(resolutions.Length - 1);
        if (resolutionDropdown != null)
        {
            resolutionDropdown.value = resolutionIndex;
        }
        SetResolution(resolutionIndex);
        
        showFPS = dataManager.GetShowFPS();
        if (fpsText != null)
        {
            fpsText.gameObject.SetActive(showFPS);
        }
        UpdateFPSButtonText();
    }
    
    public void SaveSettings()
    {
        if (dataManager == null) return;
        
        dataManager.SetFullscreen(isFullscreen);
        dataManager.SetShowFPS(showFPS);
        
        if (resolutionDropdown != null)
            dataManager.SetResolutionIndex(resolutionDropdown.value);
    }
    
    public void UpdateFPSDisplay()
    {
        if (!showFPS || fpsText == null) return;
        
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = Mathf.Ceil(fps).ToString() + " FPS";
    }
    
    private void InitializeResolutions()
    {
        resolutions = Screen.resolutions;
        if (resolutionDropdown == null) return;
        
        resolutionDropdown.ClearOptions();
        
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    
    public void ToggleFullscreen()
    {
        isFullscreen = !isFullscreen;
        Screen.fullScreen = isFullscreen;
        UpdateFullscreenButtonText();
        
        if (dataManager != null)
            dataManager.SetFullscreen(isFullscreen);
    }
    
    public void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex >= 0 && resolutionIndex < resolutions.Length)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, isFullscreen);
            
            if (dataManager != null)
                dataManager.SetResolutionIndex(resolutionIndex);
        }
    }
    
    public void ToggleFPS()
    {
        showFPS = !showFPS;
        if (fpsText != null)
        {
            fpsText.gameObject.SetActive(showFPS);
        }
        UpdateFPSButtonText();
        
        if (dataManager != null)
            dataManager.SetShowFPS(showFPS);
    }
    
    private void UpdateFullscreenButtonText()
    {
        if (fullscreenButtonText != null)
        {
            fullscreenButtonText.text = isFullscreen ? "on" : "off";
        }
    }
    
    private void UpdateFPSButtonText()
    {
        if (fpsButtonText != null)
        {
            fpsButtonText.text = showFPS ? "on" : "off";
        }
    }
}
