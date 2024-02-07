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
        if (!_isUse)
        {
            if (lineDirection == ELineBombDirectionType.None || lineDirection == ELineBombDirectionType.Max)
                lineDirection = (ELineBombDirectionType)Random.Range(1, (int)ELineBombDirectionType.Max);

            if (lineDirection == ELineBombDirectionType.Column)
            {
                _matchFinder.MatchFruits.Union(_bombManager.GetColumnFruits(_column));
                _bombManager.HitTileColumnLineBomb(_column);
            }
            else if (lineDirection == ELineBombDirectionType.Row)
            {
                _matchFinder.MatchFruits.Union(_bombManager.GetRowFruits(_row));
                _bombManager.HitTileRowLineBomb(_row);
            }

            _bombManager.LineBombDirection = ELineBombDirectionType.None;
            _isUse = true;
        }
    }
}