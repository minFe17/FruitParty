using System.Collections.Generic;
using UnityEngine;

public class EventPanel : MonoBehaviour
{
    [SerializeField] EventUIBase _shuffle;
    Dictionary<string, EventUIBase> _eventUI = new Dictionary<string, EventUIBase>();

    public Dictionary<string, EventUIBase> EventUI
    {
        get
        {
            if (_eventUI.Count == 0)
                AddEventUI();
            return _eventUI;
        }
    }

    void AddEventUI()
    {

    }
}
