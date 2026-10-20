using UnityEngine;
// 1. You must include the EventSystems namespace
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

// 2. Implement the Pointer Enter and Exit interfaces
public class ObjectHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Renderer objRenderer;
    private Color originalColor;
    public Scene scene;

    void Start()
    {
        objRenderer = GetComponent<Renderer>();
        if (objRenderer != null)
        {
            originalColor = objRenderer.material.color;
        }
    }

    // Triggered exactly when the mouse pointer enters the collider
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (objRenderer != null)
        {
            objRenderer.material.color = Color.orangeRed; // Highlight color
        }
    }

    // Triggered exactly when the mouse pointer leaves the collider
    public void OnPointerExit(PointerEventData eventData)
    {
        if (objRenderer != null)
        {
            objRenderer.material.color = originalColor; // Reset color
        }
    }

    void OnMouseDown()
    {
        SceneManager.LoadScene("Demo");
    }
}
