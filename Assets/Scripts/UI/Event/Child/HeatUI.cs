public class HeatUI : EventUIBase
{
    protected override void Start()
    {
        base.Start();
        _eventUIManager.EventUI.Add(EEventType.Heat, this);
    }

    public override void OnEventUI()
    {

    }

    public override void OffEventUI()
    {

    }
}