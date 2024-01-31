public class TyphoonUI : EventUIBase
{
    protected override void Start()
    {
        base.Start();
        _eventUIManager.EventUI.Add(EEventType.Typhoon, this);
    }

    protected override void SetSprite()
    {
        _eventSprite.sprite = _eventAtlas.GetSprite("Typhoon");
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