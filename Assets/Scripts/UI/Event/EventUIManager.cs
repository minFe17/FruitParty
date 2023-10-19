using System.Collections.Generic;
using UnityEngine;

public class EventUIManager : MonoBehaviour
{
    // ╫л╠шео
    Dictionary<EEventType, EventUIBase> _eventUI = new Dictionary<EEventType, EventUIBase>();

    public Dictionary<EEventType, EventUIBase> EventUI { get => _eventUI; }

    public void Init()
    {
        _eventUI.Clear();
    }
}
