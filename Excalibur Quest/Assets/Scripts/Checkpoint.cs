using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool isActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isActivated) return;

        if (other.CompareTag("Player"))
        {
            //GameManager.Instance.SetCheckpoint(transform.position);
            Debug.Log("Checkpoint reached at: " + transform.position);
            isActivated = true;
        }
    }
}
