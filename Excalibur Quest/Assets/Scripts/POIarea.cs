using UnityEngine;
using System.Collections;

public class POIarea : MonoBehaviour
{
    [SerializeField] private string locationName;
    [SerializeField] private bool destroyOnEnter = false;
    private bool hasEntered = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (hasEntered)
            return;

        HUDManager.Instance.UpdateLocationName(locationName);

        if (destroyOnEnter)
        {
            Destroy(gameObject);
            return;
        }

        StartCoroutine(StartCoolDownTimer());
    }

    private IEnumerator StartCoolDownTimer()
    {
        hasEntered = true;
        yield return new WaitForSeconds(120f);
        hasEntered = false;
    }
}
