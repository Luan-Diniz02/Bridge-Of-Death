using UnityEngine;

/// <summary>
/// Single Responsibility: Gerencia apenas a navegação entre menus
/// Dependency Inversion: Depende de abstrações (ISettingsManager) não de implementações concretas
/// Open/Closed: Aberto para extensão (novos gerenciadores) fechado para modificação
/// </summary>
public class MenuManager : MonoBehaviour
{
    [SerializeField] private int targetFrameRate = 60;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject audioPanel, videoPanel, creditsPanel;
    [SerializeField] private GameObject buttonAudioPressed, buttonVideoPressed, buttonCreditsPressed;
    [SerializeField] private GameObject playMenu, storeMenu, exitMenu;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject UI_Gameplay;
    
    [Header("Settings Managers")]
    [SerializeField] private AudioSettingsManager audioSettings;
    [SerializeField] private VideoSettingsManager videoSettings;
    
    private SettingsDataManager dataManager;

    private void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
        InitializeManagers();
    }
    
    private void Start()
    {
        LoadAllSettings();
    }
    
    private void Update()
    {
        // Delega atualização de FPS ao gerenciador apropriado
        if (videoSettings != null)
        {
            videoSettings.UpdateFPSDisplay();
        }
    }
    
    private void InitializeManagers()
    {
        // Cria instância do gerenciador de dados
        dataManager = new SettingsDataManager();
        
        // Injeta dependência nos gerenciadores (Dependency Injection)
        if (audioSettings != null)
        {
            audioSettings.SetDataManager(dataManager);
            audioSettings.Initialize();
        }
        
        if (videoSettings != null)
        {
            videoSettings.SetDataManager(dataManager);
            videoSettings.Initialize();
        }
    }
    
    private void LoadAllSettings()
    {
        if (audioSettings != null)
            audioSettings.LoadSettings();
            
        if (videoSettings != null)
            videoSettings.LoadSettings();
    }
    
    public void SaveAllSettings()
    {
        if (audioSettings != null)
            audioSettings.SaveSettings();
            
        if (videoSettings != null)
            videoSettings.SaveSettings();
            
        if (dataManager != null)
            dataManager.Save();
    }

    public void OpenPlayMenu()
    {
        if(playMenu != null) playMenu.SetActive(true);
        if(exitMenu != null) exitMenu.SetActive(false);
        if(storeMenu != null) storeMenu.SetActive(false);
    }

    public void OpenExitMenu()
    {
        if(exitMenu != null) exitMenu.SetActive(true);
        if(playMenu != null) playMenu.SetActive(false);
        if(storeMenu != null) storeMenu.SetActive(false);
    }

    public void OpenStoreMenu()
    {
        if(storeMenu != null) storeMenu.SetActive(true);
        if(playMenu != null) playMenu.SetActive(false);
        if(exitMenu != null) exitMenu.SetActive(false);
    }

    public void CloseExitMenu()
    {
        if(exitMenu != null) exitMenu.SetActive(false);
    }

    public void OpenSettings()
    {
        if(mainMenu != null) mainMenu.SetActive(false);
        if(settingsMenu != null) settingsMenu.SetActive(true);
        CloseAllMenus();

        OpenPanel(audioPanel);
        PressButton(buttonAudioPressed);
    }

    public void CloseSettings()
    {
        if(settingsMenu != null) settingsMenu.SetActive(false);
        if(mainMenu != null) mainMenu.SetActive(true);
    }

    private void CloseAllMenus()
    {
        if(playMenu != null) playMenu.SetActive(false);
        if(exitMenu != null) exitMenu.SetActive(false);
        if(storeMenu != null) storeMenu.SetActive(false);
    }

    public void OpenPanel(GameObject panel)
    {
        TogglePanelsVisibility(false);

        if(panel != null) panel.SetActive(true);
    }

    public void ClosePanel(GameObject panel)
    {
        if(panel != null) panel.SetActive(false);
    }

    public void PressButton(GameObject button)
    {
        ToggleButtonsVisibility(false);

        if(button != null) button.SetActive(true);
    }

    public void ShowPauseButton(bool state)
    {
        if(pauseButton != null) pauseButton.SetActive(state);
        if(UI_Gameplay != null) UI_Gameplay.SetActive(state);
        Time.timeScale = state ? 1 : 0;
    }

    private void TogglePanelsVisibility(bool state)
    {
        if(audioPanel != null) audioPanel.SetActive(state);
        if(videoPanel != null) videoPanel.SetActive(state);
        if(creditsPanel != null) creditsPanel.SetActive(state);
    }

    private void ToggleButtonsVisibility(bool state)
    {
        if(buttonAudioPressed != null) buttonAudioPressed.SetActive(state);
        if(buttonVideoPressed != null) buttonVideoPressed.SetActive(state);
        if(buttonCreditsPressed != null) buttonCreditsPressed.SetActive(state);
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
        Time.timeScale = 1;
    }
}
