using UnityEngine;

[System.Serializable]
public class AudioArray
{
    public string audioArrayName;
    public AudioClip[] audioClips;
}

public class AudioClipMatrix : MonoBehaviour
{
    public AudioArray[] audioArrays;

    public void PlayRandomAudioFromArray(string arrayName)
    {
        foreach (AudioArray audioArray in audioArrays)
        {
            if (audioArray.audioArrayName == arrayName)
            {
                if (audioArray.audioClips.Length > 0)
                {
                    SoundFXManager.Instance.PlayRandomSoundFXClip(audioArray.audioClips, transform, 1f);
                }
                break;
            }
        }
    }
}
