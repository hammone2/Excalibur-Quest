using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public static AudioMixerManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("masterVolume", Mathf.Log10(level) * 20f);
    }

    public void SetSFXVolume(float level)
    {
        audioMixer.SetFloat("sfxVolume", Mathf.Log10(level) * 20f);
    }

    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("musicVolume", Mathf.Log10(level) * 20f);
    }

    public float GetMasterVolume()
    {
        if (audioMixer.GetFloat("masterVolume", out float value))
        {
            // Convert from dB back to linear (slider) value
            return Mathf.Pow(10f, value / 20f);
        }
        else
        {
            Debug.LogWarning("Failed to get master volume. Returning default value.");
            return 1f; // Default to max volume
        }
    }

    public float GetSFXVolume()
    {
        if (audioMixer.GetFloat("sfxVolume", out float value))
        {
            return Mathf.Pow(10f, value / 20f);
        }
        else
        {
            Debug.LogWarning("Failed to get SFX volume. Returning default value.");
            return 1f;
        }
    }

    public float GetMusicVolume()
    {
        if (audioMixer.GetFloat("musicVolume", out float value))
        {
            return Mathf.Pow(10f, value / 20f);
        }
        else
        {
            Debug.LogWarning("Failed to get music volume. Returning default value.");
            return 1f;
        }
    }
}
