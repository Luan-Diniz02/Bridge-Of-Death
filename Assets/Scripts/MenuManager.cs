using Unity.AppUI.UI;
using Unity.VisualScripting;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private int targetFrameRate = 60;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject audioPanel, videoPanel, creditsPanel;

    private void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
    }

    public void OpenSettings()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        OpenPanel(audioPanel);
    }

    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void OpenPanel(GameObject panel)
    {
        audioPanel.SetActive(false);
        videoPanel.SetActive(false);
        creditsPanel.SetActive(false);

        panel.SetActive(true);
    }

    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

}
