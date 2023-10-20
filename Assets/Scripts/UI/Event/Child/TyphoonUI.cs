using UnityEngine;

public class TyphoonUI : EventUIBase
{
    RectTransform _uiTransform;
    Vector3 _startPosition;

    protected override void Start()
    {
        base.Start();
        _eventUIManager.EventUI.Add(EEventType.Typhoon, this);
        _uiTransform = GetComponent<RectTransform>();
        _startPosition = _uiTransform.position;
    }

    public override void OnEventUI()
    {
        _uiAnimator.SetBool("isTyphoon", true);
    }

    public override void OffEventUI()
    {
        _uiAnimator.SetBool("isTyphoon", false);
    }

    public override void InitEventUI()
    {
        _uiTransform.position = _startPosition;
    }
}