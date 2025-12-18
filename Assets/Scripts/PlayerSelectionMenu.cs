using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Menu de seleção de modo de jogo (Single Player / Split Screen)
/// </summary>
public class PlayerSelectionMenu : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string storeSceneName = "Menu";
    [SerializeField] private string gameplaySceneName = "Single Player";
    
    [Header("Character Selection (apenas para Split Screen)")]
    [SerializeField] private GameObject player2SelectionPanel; // Painel para escolher segundo personagem
    
    private bool isSplitScreenMode = false;
    
    /// <summary>
    /// Chamado quando o botão "Single Player" é clicado
    /// </summary>
    public void OnSinglePlayerClicked()
    {
        // Configura modo single player
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.IsTwoPlayerMode = false;
            GameSettings.Instance.SplitScreenEnabled = false;
        }
        
        Debug.Log("Modo Single Player selecionado");
        
        // Carrega cena de gameplay diretamente (usa personagem já selecionado)
        SceneManager.LoadScene(gameplaySceneName);
    }
    
    /// <summary>
    /// Chamado quando o botão "Split Screen" é clicado
    /// </summary>
    public void OnSplitScreenClicked()
    {
        // Configura modo split screen
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.IsTwoPlayerMode = true;
            GameSettings.Instance.SplitScreenEnabled = true;
        }
        
        Debug.Log("Modo Split Screen selecionado - Abrindo seleção de personagens");
        
        // Abre painel para selecionar o segundo personagem
        // Ou abre a loja em modo "selecionar segundo jogador"
        OpenPlayer2Selection();
    }
    
    /// <summary>
    /// Abre a tela de seleção do segundo personagem
    /// </summary>
    private void OpenPlayer2Selection()
    {
        // Opção 1: Mostrar painel na mesma cena
        if (player2SelectionPanel != null)
        {
            player2SelectionPanel.SetActive(true);
        }
        else
        {
            // Opção 2: Voltar para a loja em modo "selecionar segundo jogador"
            PlayerPrefs.SetInt("SelectingPlayer2", 1); // Flag temporária
            PlayerPrefs.Save();
            SceneManager.LoadScene(storeSceneName);
        }
    }
    
    /// <summary>
    /// Chamado quando o segundo personagem foi selecionado
    /// </summary>
    public void OnPlayer2Selected(string characterID, GameObject prefab)
    {
        // Salva a seleção do Player 2 separadamente
        PlayerPrefs.SetString("SelectedCharacter_Player2", characterID);
        PlayerPrefs.SetInt("SelectingPlayer2", 0); // Remove flag
        PlayerPrefs.Save();
        
        // TODO: Salvar prefab do Player 2 no CharacterSelection ou criar Player2Selection
        
        Debug.Log($"Player 2 selecionado: {characterID}");
        
        // Carrega cena de gameplay
        SceneManager.LoadScene(gameplaySceneName);
    }
    
    /// <summary>
    /// Verifica se está em modo de seleção do Player 2
    /// </summary>
    public static bool IsSelectingPlayer2()
    {
        return PlayerPrefs.GetInt("SelectingPlayer2", 0) == 1;
    }
}
