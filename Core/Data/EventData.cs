using System;
using UnityEngine;

public class EventData
{
    public EventStateType _stateType;
    public EventStateType stateType
    {
        get => _stateType;
        set
        {
            _stateType = value;
            OnStateChanged?.Invoke(_stateType);
        }
    }
    public Action<EventStateType> OnStateChanged;
    
}

