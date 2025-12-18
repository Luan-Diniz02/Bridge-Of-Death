using UnityEngine;

/// <summary>
/// Single Responsibility: Gerencia apenas o salvamento e carregamento de dados
/// </summary>
public class SettingsDataManager
{
    private const string AUDIO_VOLUME_KEY = "AudioVolume";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string FULLSCREEN_KEY = "Fullscreen";
    private const string RESOLUTION_KEY = "ResolutionIndex";
    private const string SHOW_FPS_KEY = "ShowFPS";
    
    public float GetAudioVolume(float defaultValue = 1f)
    {
        return PlayerPrefs.GetFloat(AUDIO_VOLUME_KEY, defaultValue);
    }
    
    public void SetAudioVolume(float value)
    {
        PlayerPrefs.SetFloat(AUDIO_VOLUME_KEY, value);
    }
    
    public float GetMusicVolume(float defaultValue = 1f)
    {
        return PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, defaultValue);
    }
    
    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, value);
    }
    
    public bool GetFullscreen(bool defaultValue = true)
    {
        return PlayerPrefs.GetInt(FULLSCREEN_KEY, defaultValue ? 1 : 0) == 1;
    }
    
    public void SetFullscreen(bool value)
    {
        PlayerPrefs.SetInt(FULLSCREEN_KEY, value ? 1 : 0);
    }
    
    public int GetResolutionIndex(int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(RESOLUTION_KEY, defaultValue);
    }
    
    public void SetResolutionIndex(int value)
    {
        PlayerPrefs.SetInt(RESOLUTION_KEY, value);
    }
    
    public bool GetShowFPS(bool defaultValue = false)
    {
        return PlayerPrefs.GetInt(SHOW_FPS_KEY, defaultValue ? 1 : 0) == 1;
    }
    
    public void SetShowFPS(bool value)
    {
        PlayerPrefs.SetInt(SHOW_FPS_KEY, value ? 1 : 0);
    }
    
    public void Save()
    {
        PlayerPrefs.Save();
    }
}
