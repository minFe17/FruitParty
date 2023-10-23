using UnityEngine;

public class VolcanoUI : EventUIBase
{
    protected override void Start()
    {
        base.Start();
        _eventUIManager.EventUI.Add(EEventType.Volcano, this);
    }

    public override void OnEventUI()
    {
        _uiAnimator.SetBool("isVolcano", true);
    }

    public override void OffEventUI()
    {
        _uiAnimator.SetBool("isVolcano", false);
    }

    public override void InitEventUI()
    {
        _eventUIImage.color = new Color(1f, 1f, 1f, 1f);
        _uiAnimator.SetBool("isEndEvent", true);

    }
}