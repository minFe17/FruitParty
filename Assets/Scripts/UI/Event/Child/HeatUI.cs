public class HeatUI : EventUIBase
{
    protected override void Start()
    {
        base.Start();
        _eventUIManager.EventUI.Add(EEventType.Heat, this);
    }

    public override void OnEventUI()
    {
        _uiAnimator.SetBool("isHeat", true);
    }

    public override void OffEventUI()
    {
        _uiAnimator.SetBool("isHeat", false);
    }
}