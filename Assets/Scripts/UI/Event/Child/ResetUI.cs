public class ResetUI : EventUIBase
{
    protected override void Start()
    {
        base.Start();
        _eventUIManager.EventUI.Add(EEventType.Reset, this);
    }

    protected override void SetSprite()
    {
        _eventSprite.sprite = _eventAtlas.GetSprite("Reset");
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