using UnityEngine;
using UnityEngine.UI;
using Utils;

public class SoundOptionUI : MonoBehaviour
{
    [SerializeField] Slider _bgmSlider;
    [SerializeField] Slider _sfxSlider;

    SoundManager _soundManager;

    void Start()
    {
        _soundManager = GenericSingleton<SoundManager>.Instance;

        _bgmSlider.value = _soundManager.BgmSound;
        _sfxSlider.value = _soundManager.SFXSound;
    }

    public void ContollerBGMSlider()
    {
        _soundManager.BgmSound = _bgmSlider.value;
        _soundManager.ChangeBGMVolumn();
    }

    public void ControllerSFXSlider()
    {
        _soundManager.SFXSound = _sfxSlider.value;
        _soundManager.ChangeSFXVolumn();
    }
}