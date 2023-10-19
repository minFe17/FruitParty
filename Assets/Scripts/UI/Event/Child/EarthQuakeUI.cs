public class EarthQuakeUI : EventUIBase
{
    protected override void Start()
    {
        base.Start();
        _eventUIManager.EventUI.Add(EEventType.EarthQuake, this);
    }

    public override void OnEventUI()
    {
        
    }

    public override void OffEventUI()
    {
        
    }
}