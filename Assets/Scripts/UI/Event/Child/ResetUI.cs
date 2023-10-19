public class ResetUI : EventUIBase
{
    protected override void Start()
    {
        base.Start();
        _eventUIManager.EventUI.Add(EEventType.Reset, this);
    }

    public override void OnEventUI()
    {
        
    }

    public override void OffEventUI()
    {

    }
}