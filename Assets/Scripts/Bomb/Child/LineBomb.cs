using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LineBomb : Bomb
{
    StringBuilder _stringBuilder = new StringBuilder();

    protected override void Awake()
    {
        base.Awake();
        _bombType = EBombType.LineBomb;
        if (_colorType == EColorType.Max)
            ChangeColor();
    }

    protected override void SetSprite()
    {
        _stringBuilder.Clear();
        string color = _colorType.ToString();
        _stringBuilder.Append(color);
        _stringBuilder.Append("LineBomb");
        _spriteRenderer.sprite = _spriteManager.BombAtlas.GetSprite(_stringBuilder.ToString());
    }

    public override void OnEffect()
    {
        ELineBombDirectionType lineDirection = _bombManager.LineBombDirection;
        List<Fruit> fruits;
        if (!_isUse)
        {
            if (lineDirection == ELineBombDirectionType.None || lineDirection == ELineBombDirectionType.Max)
                lineDirection = (ELineBombDirectionType)Random.Range(1, (int)ELineBombDirectionType.Max);

            if (lineDirection == ELineBombDirectionType.Column)
            {
                _bombManager.GetColumnFruits(_column, out fruits);
                _matchFinder.MatchFruits.Union(fruits);
                _bombManager.HitTileColumnLineBomb(_column);
            }
            else if (lineDirection == ELineBombDirectionType.Row)
            {
                _bombManager.GetRowFruits(_row, out fruits);
                _matchFinder.MatchFruits.Union(fruits);
                _bombManager.HitTileRowLineBomb(_row);
            }

            _bombManager.LineBombDirection = ELineBombDirectionType.None;
            _isUse = true;
        }
    }
}