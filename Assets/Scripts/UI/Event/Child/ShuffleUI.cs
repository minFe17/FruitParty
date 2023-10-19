public class ShuffleUI : EventUIBase
{
    protected override void Start()
    {
        base.Start();
        _eventUIManager.EventUI.Add(EEventType.Shuffle, this);
    }

    public override void OnEventUI()
    {
        
    }

    public override void OffEventUI()
    {

    }
}