using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;

    private bool isPaused;
    public bool toggleDisabled = false;

    public static PauseManager Instance { get; private set; }

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

    private void Start()
    {
        ResumeGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (toggleDisabled)
            return;

        if (PlayerController.Instance.IsDead()) //disable this check later if in multiplayer
            return;

        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame(bool showPauseMenu = true)
    {
        isPaused = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (showPauseMenu)
        {
            pauseMenu.SetActive(true);
        }
    }

    public void ResumeGame(bool showPauseMenu = true)
    {
        isPaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (showPauseMenu)
        {
            pauseMenu.SetActive(false);
        }
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    public void DisablePause()
    {
        toggleDisabled = true;
    }

    public void EnablePause()
    {
        toggleDisabled = false;
    }
}