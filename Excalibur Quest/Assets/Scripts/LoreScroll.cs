using UnityEngine;

public class LoreScroll : MonoBehaviour
{
    [TextArea(3, 10)]
    [SerializeField] private string loreContent;
    bool isPlayerInRange = false;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E)) //replace with input action later
        {
            PauseManager.Instance.PauseGame(false);
            HUDManager.Instance.ShowLoreScroll(loreContent);
        }
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
