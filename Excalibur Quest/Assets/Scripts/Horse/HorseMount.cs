using UnityEngine;
using UnityEngine.InputSystem;

public class HorseMount : MonoBehaviour
{
    public HorseController horse;
    public InputActionReference interactAction;

    public Transform mountPoint;
    public Transform dismountPoint;

    public CharacterController playerController;
    public GameObject playerVisual;

    public float mountDistance = 3f;

    private bool mounted;

    private void OnEnable()
    {
        interactAction.action.Enable();
        interactAction.action.performed += OnInteract;
    }

    private void OnDisable()
    {
        interactAction.action.performed -= OnInteract;
        interactAction.action.Disable();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (mounted)
        {
            Dismount();
            return;
        }

        float distance = Vector3.Distance(
            transform.position,
            horse.transform.position
        );

        if (distance <= mountDistance)
            Mount();
    }

    private void Mount()
    {
        mounted = true;

        playerController.enabled = false;

        transform.SetParent(mountPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        horse.SetRidden(true);
    }

    private void Dismount()
    {
        mounted = false;

        transform.SetParent(null);

        transform.position = dismountPoint.position;
        transform.rotation = dismountPoint.rotation;

        playerController.enabled = true;

        horse.SetRidden(false);
    }
}