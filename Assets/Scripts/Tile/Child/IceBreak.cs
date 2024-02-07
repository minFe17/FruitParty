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
        for (int i = 1; i <= _iceSprites.Count; i++)
        {
            _stringBuilder.Clear();
            _stringBuilder.Append(_breakIceType.ToString());
            _stringBuilder.Append(i);
            _iceSprites[i - 1].sprite = tileAtlas.GetSprite(_stringBuilder.ToString());
        }
    }
}