using UnityEngine;

public class HUDManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private Animator checkpointText;
    public Healthbar healthBar;


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
}
