using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static LoreScroll;

public class HUDManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private Animator checkpointText;
    [SerializeField] private GameObject itemPickupTextPrefab;
    [SerializeField] private Transform itemPickupTextLayout;
    [SerializeField] private GameObject locationEntered;
    public Healthbar healthBar;

    [Header("Lore Scroll")]
    private LoreScroll currentLoreScroll;
    [SerializeField] private GameObject loreScrollUI;
    [SerializeField] private TextMeshProUGUI loreScrollText;

    public static HUDManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnPlayerDeath()
    {
        deathScreen.GetComponent<Animator>().SetTrigger("Death");
    }

    public void OnCheckpointReached()
    {
        checkpointText.SetTrigger("Checkpoint");
    }

    public void ShowItemPickupText(string itemName, bool showPrefix = true)
    {
        GameObject itemPickupText = Instantiate(itemPickupTextPrefab, itemPickupTextLayout);

        if (showPrefix)
        {
            itemPickupText.GetComponent<TextMeshProUGUI>().SetText("Picked up " + itemName);
        }
        else
        {
            itemPickupText.GetComponent<TextMeshProUGUI>().SetText(itemName);
        }
    }

    public void UpdateLocationName(string locationName)
    {
        locationEntered.GetComponentInChildren<TextMeshProUGUI>().SetText(locationName);
        locationEntered.GetComponent<Animator>().SetTrigger("Enter");
    }

    public void ShowLoreScroll(string loreContent, LoreScroll loreScroll)
    {
        currentLoreScroll = loreScroll;
        currentLoreScroll.scrollState = ScrollState.Reading;
        loreScrollUI.SetActive(true);
        loreScrollText.SetText(loreContent);
        PauseManager.Instance.toggleDisabled = true;
        PauseManager.Instance.PauseGame(false);
    }

    public void HideLoreScroll()
    {
        currentLoreScroll.scrollState = ScrollState.NotReading;
        currentLoreScroll = null;
        loreScrollUI.SetActive(false);
        PauseManager.Instance.toggleDisabled = false;
        PauseManager.Instance.ResumeGame(false);
    }

    public void OnQuitToTitleButtonPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScreen");
    }
}
