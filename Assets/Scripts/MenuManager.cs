using Unity.AppUI.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private int targetFrameRate = 60;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject audioPanel, videoPanel, creditsPanel;
    [SerializeField] private GameObject buttonAudioPressed, buttonVideoPressed, buttonCreditsPressed;
    [SerializeField] private GameObject playMenu, exitMenu;
    
    [Header("Audio Settings")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider audioVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    
    [Header("Video Settings")]
    [SerializeField] private UnityEngine.UI.Button fullscreenButton;
    [SerializeField] private UnityEngine.UI.Button fpsButton;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TextMeshProUGUI fullscreenButtonText;
    [SerializeField] private TextMeshProUGUI fpsButtonText;
    [SerializeField] private TextMeshProUGUI fpsText;
    
    private Resolution[] resolutions;
    private bool isFullscreen = true;
    private bool showFPS = false;
    private float deltaTime = 0.0f;

    private void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
        InitializeSettings();
    }
    
    private void Start()
    {
        LoadSettings();
    }
    
    private void Update()
    {
        if (showFPS)
        {
            UpdateFPSDisplay();
        }
    }
    
    private void InitializeSettings()
    {
        // Inicializa resoluções disponíveis
        resolutions = Screen.resolutions;
        if (resolutionDropdown != null)
        {
            resolutionDropdown.ClearOptions();
            
            System.Collections.Generic.List<string> options = new System.Collections.Generic.List<string>();
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
        
        // Configura listeners
        if (audioVolumeSlider != null)
            audioVolumeSlider.onValueChanged.AddListener(SetAudioVolume);
            
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
            
        if (fullscreenButton != null)
            fullscreenButton.onClick.AddListener(ToggleFullscreen);
            
        if (resolutionDropdown != null)
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
            
        if (fpsButton != null)
            fpsButton.onClick.AddListener(ToggleFPS);
    }

    public void OpenPlayMenu()
    {
        playMenu.SetActive(true);
        exitMenu.SetActive(false);
    }

    public void OpenExitMenu()
    {
        exitMenu.SetActive(true);
        playMenu.SetActive(false);
    }

    public void CloseExitMenu()
    {
        exitMenu.SetActive(false);
    }

    public void OpenSettings()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);

        OpenPanel(audioPanel);
        PressButton(buttonAudioPressed);
    }

    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void OpenPanel(GameObject panel)
    {
        TogglePanelsVisibility(false);

        panel.SetActive(true);
    }

    public void PressButton(GameObject button)
    {
        ToggleButtonsVisibility(false);

        button.SetActive(true);
    }

    private void TogglePanelsVisibility(bool state)
    {
        audioPanel.SetActive(state);
        videoPanel.SetActive(state);
        creditsPanel.SetActive(state);
    }

    private void ToggleButtonsVisibility(bool state)
    {
        buttonAudioPressed.SetActive(state);
        buttonVideoPressed.SetActive(state);
        buttonCreditsPressed.SetActive(state);
    }

    public void ExitGame()
    {
        if(Application.isEditor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }
    }

    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
    
    // ========== MÉTODOS DE ÁUDIO ==========
    
    public void SetAudioVolume(float volume)
    {
        if (audioMixer != null)
        {
            // Converte de 0-1 para -80dB a 0dB
            float dB = volume > 0 ? 20f * Mathf.Log10(volume) : -80f;
            audioMixer.SetFloat("AudioVolume", dB);
            PlayerPrefs.SetFloat("AudioVolume", volume);
        }
    }
    
    public void SetMusicVolume(float volume)
    {
        if (audioMixer != null)
        {
            // Converte de 0-1 para -80dB a 0dB
            float dB = volume > 0 ? 20f * Mathf.Log10(volume) : -80f;
            audioMixer.SetFloat("MusicVolume", dB);
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }
    }
    
    // ========== MÉTODOS DE VÍDEO ==========
    
    public void ToggleFullscreen()
    {
        isFullscreen = !isFullscreen;
        Screen.fullScreen = isFullscreen;
        UpdateFullscreenButtonText();
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }
    
    private void UpdateFullscreenButtonText()
    {
        if (fullscreenButtonText != null)
        {
            fullscreenButtonText.text = isFullscreen ? "on" : "off";
        }
    }
    
    public void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex >= 0 && resolutionIndex < resolutions.Length)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, isFullscreen);
            PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
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
        PlayerPrefs.SetInt("ShowFPS", showFPS ? 1 : 0);
    }
    
    private void UpdateFPSButtonText()
    {
        if (fpsButtonText != null)
        {
            fpsButtonText.text = showFPS ? "on" : "off";
        }
    }
    
    private void UpdateFPSDisplay()
    {
        if (fpsText != null)
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;
            fpsText.text = Mathf.Ceil(fps).ToString() + " FPS";
        }
    }
    
    // ========== CARREGAR/SALVAR CONFIGURAÇÕES ==========
    
    private void LoadSettings()
    {
        // Carrega volume de áudio
        if (audioVolumeSlider != null)
        {
            float audioVolume = PlayerPrefs.GetFloat("AudioVolume", 1f);
            audioVolumeSlider.value = audioVolume;
            SetAudioVolume(audioVolume);
        }
        
        // Carrega volume de música
        if (musicVolumeSlider != null)
        {
            float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            musicVolumeSlider.value = musicVolume;
            SetMusicVolume(musicVolume);
        }
        
        // Carrega fullscreen
        isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        Screen.fullScreen = isFullscreen;
        UpdateFullscreenButtonText();
        
        // Carrega resolução
        if (resolutionDropdown != null)
        {
            int resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", resolutions.Length - 1);
            resolutionDropdown.value = resolutionIndex;
            SetResolution(resolutionIndex);
        }
        
        // Carrega FPS
        showFPS = PlayerPrefs.GetInt("ShowFPS", 0) == 1;
        if (fpsText != null)
        {
            fpsText.gameObject.SetActive(showFPS);
        }
        UpdateFPSButtonText();
    }
    
    public void SaveSettings()
    {
        PlayerPrefs.Save();
    }

}
