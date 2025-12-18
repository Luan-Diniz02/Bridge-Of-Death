using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// Single Responsibility: Gerencia apenas configurações de áudio
/// Open/Closed: Pode ser estendido sem modificar o código
/// </summary>
public class AudioSettingsManager : MonoBehaviour, ISettingsManager
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider audioVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    
    private SettingsDataManager dataManager;
    private const string AUDIO_PARAM = "AudioVolume";
    private const string MUSIC_PARAM = "MusicVolume";
    
    public void SetDataManager(SettingsDataManager manager)
    {
        dataManager = manager;
    }
    
    public void Initialize()
    {
        if (audioVolumeSlider != null)
            audioVolumeSlider.onValueChanged.AddListener(SetAudioVolume);
            
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
    }
    
    public void LoadSettings()
    {
        if (dataManager == null) return;
        
        float audioVolume = dataManager.GetAudioVolume();
        if (audioVolumeSlider != null)
        {
            audioVolumeSlider.value = audioVolume;
        }
        SetAudioVolume(audioVolume);
        
        float musicVolume = dataManager.GetMusicVolume();
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = musicVolume;
        }
        SetMusicVolume(musicVolume);
    }
    
    public void SaveSettings()
    {
        if (dataManager == null) return;
        
        if (audioVolumeSlider != null)
            dataManager.SetAudioVolume(audioVolumeSlider.value);
            
        if (musicVolumeSlider != null)
            dataManager.SetMusicVolume(musicVolumeSlider.value);
    }
    
    public void SetAudioVolume(float volume)
    {
        if (audioMixer != null)
        {
            float dB = volume > 0 ? 20f * Mathf.Log10(volume) : -80f;
            audioMixer.SetFloat(AUDIO_PARAM, dB);
        }
        
        if (dataManager != null)
            dataManager.SetAudioVolume(volume);
    }
    
    public void SetMusicVolume(float volume)
    {
        if (audioMixer != null)
        {
            float dB = volume > 0 ? 20f * Mathf.Log10(volume) : -80f;
            audioMixer.SetFloat(MUSIC_PARAM, dB);
        }
        
        if (dataManager != null)
            dataManager.SetMusicVolume(volume);
    }
}
