public class TyphoonUI : EventUIBase
{
    protected override void Start()
    {
        base.Start();
        _eventUIManager.EventUI.Add(EEventType.Typhoon, this);
    }

    public override void OnEventUI()
    {
        _uiAnimator.SetBool("isTyphoon", true);
        _uiAnimator.SetBool("isEndEvent", false);
    }

    public override void OffEventUI()
    {
        _uiAnimator.SetBool("isTyphoon", false);
    }

    public override void InitEventUI()
    {
        _uiAnimator.SetBool("isEndEvent", true);
    }
}