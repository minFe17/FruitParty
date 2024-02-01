using System.Linq;
using System.Text;
using UnityEngine;

public class LineBomb : Bomb
{
    StringBuilder _stringBuilder = new StringBuilder();

    protected override void Start()
    {
        base.Start();
        _bombType = EBombType.LineBomb;
    }

    protected override void SetSprite()
    {
        string color = _colorType.ToString();
        _stringBuilder.Append(color);
        _stringBuilder.Append("LineBomb");
        _spriteRenderer.sprite = _spriteManager.BombAtlas.GetSprite(_stringBuilder.ToString());
        Debug.Log(_stringBuilder);
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