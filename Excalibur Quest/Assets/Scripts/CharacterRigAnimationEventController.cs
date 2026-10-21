using UnityEngine;

[System.Serializable]
public class CharacterRigAnimationEvent
{
    public string eventName;
    public UnityEngine.Events.UnityEvent eventAction;
}

public class CharacterRigAnimationEventController : MonoBehaviour
{
    [SerializeField] private CharacterRigAnimationEvent[] animationEvents;

    public void TriggerAnimationEvent(string eventName)
    {
        foreach (CharacterRigAnimationEvent animationEvent in animationEvents)
        {
            if (animationEvent.eventName == eventName)
            {
                animationEvent.eventAction?.Invoke();
                return;
            }
        }

        Debug.LogWarning($"Animation event '{eventName}' was not found.", this);
    }
}
