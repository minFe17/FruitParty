public class MarketDayUI : EventUIBase
{
    protected override void Start()
    {
        base.Start();
        _eventUIManager.EventUI.Add(EEventType.MarketDay, this);
    }

    public override void OnEventUI()
    {

    }

    public override void OffEventUI()
    {

    }
}