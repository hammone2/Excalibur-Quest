using UnityEngine;

public class CastleGate : MonoBehaviour
{
    private const int slots = 3;
    private int keysInserted = 0;
    [SerializeField] private float openSpeed = 1f;
    [SerializeField] private float yTarget = 5f;


    private void Awake()
    {
        KeyHole[] keyHoles = GetComponentsInChildren<KeyHole>();
        for (int i = 0; i < keyHoles.Length; i++)
        {
            keyHoles[i].OnKeyInserted.AddListener(AddKey);
        }
    }


    public void AddKey()
    {
        keysInserted++;
        HUDManager.Instance.ShowItemPickupText("Key Inserted! " + keysInserted + "/" + slots, false);

        if (keysInserted >= slots)
        {
            StartCoroutine(OpenGate());
        }
    }

    private System.Collections.IEnumerator OpenGate()
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(startPosition.x, startPosition.y - yTarget, startPosition.z);
        float elapsedTime = 0f;
        while (elapsedTime < openSpeed)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / openSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }
}
