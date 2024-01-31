using UnityEngine;

public class ShuffleUI : EventUIBase
{
    protected override void Start()
    {
        base.Start();
        _eventUIManager.EventUI.Add(EEventType.Shuffle, this);
    }

    protected override void SetSprite()
    {
        _eventSprite.sprite = _eventAtlas.GetSprite("Shuffle");
    }

    public override void OnEventUI()
    {
        _uiAnimator.SetBool("isShuffle", true);
        _uiAnimator.SetBool("isEndEvent", false);
    }

    public override void OffEventUI()
    {
        _uiAnimator.SetBool("isShuffle", false);
    }

    public override void InitEventUI()
    {
        _eventUIImage.enabled = false;
        _eventUIImage.fillAmount = 0f;
        _eventUIImage.color = new Color(1f, 1f, 1f, 1f);
        _uiAnimator.SetBool("isEndEvent", true);
    }
}