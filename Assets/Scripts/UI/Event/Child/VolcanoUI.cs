public class VolcanoUI : EventUIBase
{
    protected override void Start()
    {
        base.Start();
        _eventUIManager.EventUI.Add(EEventType.Volcano, this);
    }

    public override void OnEventUI()
    {

    }

    public override void OffEventUI()
    {

    }
}