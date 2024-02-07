using UnityEngine;
using UnityEngine.UI;
using Utils;

public class Background : MonoBehaviour
{
    [SerializeField] Image _backgroundSprite;

    SpriteManager _spriteMaanger;

    void Start()
    {
        _spriteMaanger = GenericSingleton<SpriteManager>.Instance;
        SetSprite();
    }

    private void SetSprite()
    {
        _backgroundSprite.sprite = _spriteMaanger.BackgroundSprite;
    }
}