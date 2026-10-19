using UnityEngine;
using UnityEngine.Events;

public class StartFinsihLine : MonoBehaviour
{
    public UnityEvent OnCrossed;
    private bool crossed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (crossed) return;

        if (other.CompareTag("Player"))
        {
            crossed = true;
            OnCrossed.Invoke();
        }
    }
}
