using UnityEngine;

public class MarketDayUI : EventUIBase
{
    RectTransform _uiTransform;

    protected override void Start()
    {
        base.Start();
        _eventUIManager.EventUI.Add(EEventType.MarketDay, this);
        _uiTransform = GetComponent<RectTransform>();
    }

    public override void OnEventUI()
    {
        _uiAnimator.SetBool("isMarketDay", true);
        _uiAnimator.SetBool("isEndEvent", false);
    }

    public override void OffEventUI()
    {
        _uiAnimator.SetBool("isMarketDay", false);
    }

    public override void InitEventUI()
    {
        _uiAnimator.SetBool("isEndEvent", true);
    }
}