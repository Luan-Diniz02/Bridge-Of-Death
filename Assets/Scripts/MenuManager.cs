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
    [SerializeField] private GameObject playMenu, exitMenu;
    
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
}
