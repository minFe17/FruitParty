using UnityEngine;

public class MarketDayUI : EventUIBase
{
    RectTransform _uiTransform;
    Vector3 _startPosition;

    protected override void Start()
    {
        base.Start();
        _eventUIManager.EventUI.Add(EEventType.MarketDay, this);
        _uiTransform = GetComponent<RectTransform>();
        _startPosition = _uiTransform.position;
    }

    public override void OnEventUI()
    {
        _uiAnimator.SetBool("isMarketDay", true);
    }

    public override void OffEventUI()
    {
        _uiAnimator.SetBool("isMarketDay", false);
    }

    public override void InitEventUI()
    {
        _uiTransform.position = _startPosition;
    }
}