using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class TitlePanel : MonoBehaviour
{
    [Header("Sprite")]
    [SerializeField] List<Image> _ropeSprites;
    [SerializeField] Image _WoodBoardSprite;
    [SerializeField] Image _strawberrySprite;
    [SerializeField] Image _watermelonSprite;

    SpriteManager _spriteManager;

    void Start()
    {
        _spriteManager = GenericSingleton<SpriteManager>.Instance;
        SetSprite();
    }

    void SetSprite()
    {
        for (int i = 0; i < _ropeSprites.Count; i++)
            _ropeSprites[i].sprite = _spriteManager.UIAtlas.GetSprite("Rope");
        _WoodBoardSprite.sprite = _spriteManager.UIAtlas.GetSprite("WoodBoard");
        _strawberrySprite.sprite = _spriteManager.FruitAtlas.GetSprite("Strawberry");
        _watermelonSprite.sprite = _spriteManager.FruitAtlas.GetSprite("Watermelon");
    }
}