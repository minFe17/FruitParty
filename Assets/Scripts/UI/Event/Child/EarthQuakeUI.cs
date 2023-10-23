using UnityEngine;

public class EarthQuakeUI : EventUIBase
{
    RectTransform _uiTranform;
    protected override void Start()
    {
        base.Start();
        _eventUIManager.EventUI.Add(EEventType.EarthQuake, this);
        _uiTranform = GetComponent<RectTransform>();
    }

    public override void OnEventUI()
    {
        _uiAnimator.SetBool("isEarthQuake", true);
        _uiAnimator.SetBool("isEndEvent", false);
    }

    public override void OffEventUI()
    {
        _uiAnimator.SetBool("isEarthQuake", false);
    }

    public override void InitEventUI()
    {
        _uiTranform.localScale.Set(0f, 0f, 1f);
        _eventUIImage.enabled = false;
        _eventUIImage.fillAmount = 1f;
        _uiAnimator.SetBool("isEndEvent", true);
    }
}