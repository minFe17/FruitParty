using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using Utils;

public abstract class EventUIBase : MonoBehaviour
{
    [SerializeField] protected Image _eventSprite;

    protected EventUIManager _eventUIManager;
    protected Image _eventUIImage;
    protected Animator _uiAnimator;
    protected SpriteAtlas _eventAtlas;

    protected virtual void Start()
    {
        _eventUIManager = GenericSingleton<EventUIManager>.Instance;
        _uiAnimator = GenericSingleton<UIManager>.Instance.UI.UIAnimator;
        _eventUIImage = GetComponent<Image>();
        _eventAtlas = GenericSingleton<SpriteManager>.Instance.EventAtlas;
        SetSprite();
    }
    protected abstract void SetSprite();
    public abstract void OnEventUI();
    public abstract void OffEventUI();

    public virtual void InitEventUI() { }
}