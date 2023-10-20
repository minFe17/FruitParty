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
    }

    public override void OffEventUI()
    {
        _uiAnimator.SetBool("isEarthQuake", false);
    }

    public override void InitEventUI()
    {
        _eventUIImage.enabled = false;
        _eventUIImage.fillAmount = 1f;
        _uiTranform.localScale = new Vector3(0.1f, 0.1f);
        Debug.Log(1);
    }
}