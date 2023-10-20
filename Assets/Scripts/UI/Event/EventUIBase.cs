using UnityEngine;
using UnityEngine.UI;
using Utils;

public abstract class EventUIBase : MonoBehaviour
{
    protected EventUIManager _eventUIManager;
    protected Image _eventUIImage;
    protected Animator _uiAnimator;

    protected virtual void Start()
    {
        _eventUIManager = GenericSingleton<EventUIManager>.Instance;
        _uiAnimator = GenericSingleton<UIManager>.Instance.UI.UIAnimator;
        _eventUIImage = GetComponent<Image>();
    }

    public abstract void OnEventUI();
    public abstract void OffEventUI();
    public abstract void InitEventUI();
}