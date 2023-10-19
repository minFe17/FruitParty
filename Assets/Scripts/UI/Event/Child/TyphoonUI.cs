public class TyphoonUI : EventUIBase
{
    protected override void Start()
    {
        base.Start();
        _eventUIManager.EventUI.Add(EEventType.Typhoon, this);
    }

    public override void OnEventUI()
    {

    }

    public override void OffEventUI()
    {

    }
}