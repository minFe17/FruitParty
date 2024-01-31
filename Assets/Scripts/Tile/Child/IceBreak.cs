using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.U2D;

public class IceBreak : MonoBehaviour
{
    [SerializeField] List<SpriteRenderer> _iceSprites;
    [SerializeField] EBreakIceType _breakIceType;

    StringBuilder _stringBuilder = new StringBuilder();

    public void SetSprite(SpriteAtlas tileAtlas)
    {
        for (int i = 0; i < _iceSprites.Count; i++)
        {
            _stringBuilder.Append(_breakIceType.ToString());
            _stringBuilder.Append(i);
            _iceSprites[i].sprite = tileAtlas.GetSprite(_stringBuilder.ToString());
        }
    }
}

public enum EBreakIceType
{
    FirstBreakIce,
    SecondBreakIce,
    ThirdBreakIce,
    Max,
}