using System.Collections;
using UnityEngine;

public class LoreScroll : MonoBehaviour
{
    [TextArea(3, 10)]
    [SerializeField] private string loreContent;
    bool isPlayerInRange = false;

    [SerializeField] private AudioClip[] interactSounds;

    public enum ScrollState
    {
        Reading,
        NotReading
    }

    public ScrollState scrollState;

    private void Awake()
    {
        scrollState = ScrollState.NotReading;
    }

    void Update()
    {
        switch (scrollState)
        {
            case ScrollState.Reading:
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E)) //replace with input action later
                {
                    HUDManager.Instance.HideLoreScroll();

                    // Prevent the same Escape press from opening the pause menu.
                    PauseManager.Instance.toggleDisabled = true;
                    StartCoroutine(ReenablePauseToggle());
                }
                break;

            case ScrollState.NotReading:
                if (PauseManager.Instance.IsPaused())
                    return;

                if (isPlayerInRange && Input.GetKeyDown(KeyCode.E)) //replace with input action later
                {
                    HUDManager.Instance.ShowLoreScroll(loreContent, this);
                    if (interactSounds.Length > 0)
                    {
                        SoundFXManager.Instance.PlayRandomSoundFXClip(interactSounds, transform, 1f);
                    }
                }
                break;
        }
    }

    private IEnumerator ReenablePauseToggle()
    {
        yield return null;
        PauseManager.Instance.toggleDisabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
