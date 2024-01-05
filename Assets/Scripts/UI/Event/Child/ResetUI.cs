public class ResetUI : EventUIBase
{
    protected override void Start()
    {
        base.Start();
        _eventUIManager.EventUI.Add(EEventType.Reset, this);
    }

    public override void OnEventUI()
    {
        _uiAnimator.SetBool("isReset", true);
    }

    public override void OffEventUI()
    {
        _uiAnimator.SetBool("isReset", false);
    }
}