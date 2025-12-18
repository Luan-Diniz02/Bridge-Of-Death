using Unity.AppUI.UI;
using Unity.VisualScripting;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private int targetFrameRate = 60;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject audioPanel, videoPanel, creditsPanel;
    [SerializeField] private GameObject buttonAudioPressed, buttonVideoPressed, buttonCreditsPressed;
    [SerializeField] private GameObject playMenu, exitMenu;

    private void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
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
