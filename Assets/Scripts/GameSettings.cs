using UnityEngine;

/// <summary>
/// Configurações globais do jogo (ScriptableObject)
/// </summary>
[CreateAssetMenu(fileName = "GameSettings", menuName = "Game/Settings")]
public class GameSettings : ScriptableObject
{
    [Header("Multiplayer Settings")]
    [SerializeField] private bool isTwoPlayerMode = false;
    [SerializeField] private bool splitScreenEnabled = false;
    
    [Header("Player Configuration")]
    [SerializeField] private int maxPlayers = 2;
    
    public bool IsTwoPlayerMode
    {
        get => isTwoPlayerMode;
        set => isTwoPlayerMode = value;
    }
    
    public bool SplitScreenEnabled
    {
        get => splitScreenEnabled;
        set => splitScreenEnabled = value;
    }
    
    public int MaxPlayers => maxPlayers;
    
    // Singleton instance
    private static GameSettings instance;
    public static GameSettings Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GameSettings>("GameSettings");
                if (instance == null)
                {
                    Debug.LogWarning("GameSettings não encontrado em Resources. Criando configurações padrão.");
                    instance = CreateInstance<GameSettings>();
                }
            }
            return instance;
        }
    }
}
