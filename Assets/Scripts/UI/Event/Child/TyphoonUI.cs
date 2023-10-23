using UnityEngine;

public class TyphoonUI : EventUIBase
{
    RectTransform _uiTransform;

    protected override void Start()
    {
        base.Start();
        _eventUIManager.EventUI.Add(EEventType.Typhoon, this);
        _uiTransform = GetComponent<RectTransform>();
    }

    public override void OnEventUI()
    {
        _uiAnimator.SetBool("isTyphoon", true);
        _uiAnimator.SetBool("isEndEvent", false);
    }

    public override void OffEventUI()
    {
        _uiAnimator.SetBool("isTyphoon", false);
    }

    public override void InitEventUI()
    {
        _uiAnimator.SetBool("isEndEvent", true);
    }
}