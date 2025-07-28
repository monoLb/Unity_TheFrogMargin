using UnityEngine;
using UnityEngine.Events;

public class BaseEventListener<T> : MonoBehaviour
{
    public BaseEventSO<T> eventSO;
    public UnityEvent<T> reponse;

    private void OnEnable()
    {
        if (eventSO != null)
            eventSO.onEvent += EventRaised;
    }

    private void OnDisable()
    {
        if(eventSO != null)
            eventSO.onEvent -= EventRaised;
    }

    private void EventRaised(T value)
    {
        reponse?.Invoke(value);
    }

}
