public class MarketDayUI : EventUIBase
{
    protected override void Start()
    {
        base.Start();
        _eventUIManager.EventUI.Add(EEventType.MarketDay, this);
    }

    protected override void SetSprite()
    {
        _eventSprite.sprite = _eventAtlas.GetSprite("MarketDay");
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