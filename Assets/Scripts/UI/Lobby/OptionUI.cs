using UnityEngine;
using UnityEngine.UI;
using Utils;

public class OptionUI : MonoBehaviour
{
    [SerializeField] Animator _animator;

    [Header("Sprite")]
    [SerializeField] Image _panelSprite;
    [SerializeField] Image _okButtonSprite;

    SoundManager _soundManager;
    AudioClipManager _audioClipManager;
    CSVManager _csvManager;
    SpriteManager _spriteManager;

    void Start()
    {
        _soundManager = GenericSingleton<SoundManager>.Instance;
        _csvManager = GenericSingleton<CSVManager>.Instance;
        _audioClipManager = GenericSingleton<AudioClipManager>.Instance;
        _spriteManager = GenericSingleton<SpriteManager>.Instance;
        SetSprite();
    }

    void SetSprite()
    {
        _panelSprite.sprite = _spriteManager.UIAtlas.GetSprite("Option Panel");
        _okButtonSprite.sprite = _spriteManager.UIAtlas.GetSprite("Button");
    }

    public void CloseOption()
    {
        _soundManager.PlaySFX(_audioClipManager.ButtonSfX);
        _animator.SetBool("isOpenOption", false);
        _csvManager.WriteSoundData();
    }
}