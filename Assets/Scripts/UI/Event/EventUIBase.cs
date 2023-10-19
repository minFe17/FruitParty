using UnityEngine;
using Utils;

public abstract class EventUIBase : MonoBehaviour
{
    protected EventUIManager _eventUIManager;

    protected virtual void Start()
    {
        _eventUIManager = GenericSingleton<EventUIManager>.Instance;
    }

    public abstract void OnEventUI();
    public abstract void OffEventUI();
}