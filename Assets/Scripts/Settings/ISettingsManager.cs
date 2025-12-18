using UnityEngine;

/// <summary>
/// Interface base para gerenciadores de configurações
/// Princípio: Interface Segregation e Dependency Inversion
/// </summary>
public interface ISettingsManager
{
    void Initialize();
    void LoadSettings();
    void SaveSettings();
}
