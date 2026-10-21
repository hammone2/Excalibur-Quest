using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance;
    [SerializeField] private AudioSource soundFXObject;


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

    public void PlaySoundFXClip(AudioClip clip, Transform spawnPoint, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawnPoint.position, Quaternion.identity);
        audioSource.clip = clip;
        audioSource.volume = volume;

        audioSource.Play();
        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayRandomSoundFXClip(AudioClip[] clips, Transform spawnPoint, float volume)
    {
        int index = Random.Range(0, clips.Length);

        PlaySoundFXClip(clips[index], spawnPoint, volume);
    }
}
