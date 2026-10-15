using TMPro;
using UnityEngine;

public class CartSceneManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI killsText;

    private float elapsedTime = 0f;
    private int kills = 0;
    private bool timerRunning = false;

    void Start()
    {
        UpdateTimerText(0f);
    }

    void Update()
    {
        if (timerRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerText(elapsedTime);
        }
    }

    public void StartTimer()
    {
        elapsedTime = 0f;
        timerRunning = true;
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    public void UpdateTimerText(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time - minutes * 60);
        timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
    }

    public void UpdateKillsText()
    {
        kills++;
        killsText.text = "Kills: " + kills.ToString();
    }
}
