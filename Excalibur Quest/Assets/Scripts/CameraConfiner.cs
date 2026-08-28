using Unity.Cinemachine;
using UnityEngine;

public class CameraConfiner : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered camera confinement zone");
            PlayerController playerController = other.GetComponent<PlayerController>();
            // Get the CinemachineConfiner component from the camera
            CinemachineConfiner3D confiner = playerController.freeLookCamera.GetComponent<CinemachineConfiner3D>();
            if (confiner != null)
            {
                confiner.GetComponent<CinemachineDecollider>().enabled = false;
                confiner.BoundingVolume = GetComponent<Collider>();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited camera confinement zone");
            PlayerController playerController = other.GetComponent<PlayerController>();
            // Get the CinemachineConfiner component from the camera
            CinemachineConfiner3D confiner = playerController.freeLookCamera.GetComponent<CinemachineConfiner3D>();
            if (confiner != null)
            {
                // Reset the confining shape to null or default
                confiner.GetComponent<CinemachineDecollider>().enabled = true;
                confiner.BoundingVolume = null;
            }
        }
    }
}
