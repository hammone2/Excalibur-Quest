using UnityEngine;

public class TemporaryRestartScript : MonoBehaviour
{
    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("StartScreen");
    }
}
