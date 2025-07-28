using UnityEngine;
using UnityEngine.Events;

public class BaseEventSO<T> : ScriptableObject
{
    [TextArea]
    public string description;
    
    public UnityAction<T> onEvent;
    public string lastSender;

    public void RaiseEvent(T value, Object sender)
    {
        onEvent?.Invoke(value);
        lastSender = sender?.ToString();
    }
}
