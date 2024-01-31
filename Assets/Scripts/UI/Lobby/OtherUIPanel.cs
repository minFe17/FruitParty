using UnityEngine;
using UnityEngine.UI;
using Utils;

public class OtherUIPanel : MonoBehaviour
{
    [SerializeField] Image _playButtonSprite;
    [SerializeField] Image _optionButtonSprite;

    SpriteManager _spriteManager;

    void Start()
    {
        _spriteManager = GenericSingleton<SpriteManager>.Instance;
        SetSprite();
    }

    void SetSprite()
    {
        _playButtonSprite.sprite = _spriteManager.UIAtlas.GetSprite("Button");
        _optionButtonSprite.sprite = _spriteManager.UIAtlas.GetSprite("Option Button");
    }
}