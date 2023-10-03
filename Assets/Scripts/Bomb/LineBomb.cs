using System.Linq;
using UnityEngine;

public class LineBomb : Bomb
{
    bool _isUse;
    protected override void Awake()
    {
        base.Awake();
        _bombType = EBombType.LineBomb;
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
                _bombManager.HitConcreteColumnLineBomb(_column);
            }
            else if (lineDirection == ELineBombDirectionType.Row)
            {
                _matchFinder.MatchFruits.Union(_bombManager.GetRowFruits(_row));
                _bombManager.HitConcreteRowLineBomb(_row);
            }

            _bombManager.LineBombDirection = ELineBombDirectionType.None;
            _isUse = true;
        }
    }
}