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
        if(!_isUse)
        {
            if (_matchFinder.LineBombDirection == ELineBombDirectionType.None || _matchFinder.LineBombDirection == ELineBombDirectionType.Max)
                _matchFinder.LineBombDirection = (ELineBombDirectionType)Random.Range(1, (int)ELineBombDirectionType.Max);

            if (_matchFinder.LineBombDirection == ELineBombDirectionType.Column)
                _matchFinder.MatchFruits.Union(_matchFinder.GetColumnFruits(_column));
            else if (_matchFinder.LineBombDirection == ELineBombDirectionType.Row)
                _matchFinder.MatchFruits.Union(_matchFinder.GetRowFruits(_row));

            _matchFinder.LineBombDirection = ELineBombDirectionType.None;

            _isUse = true;
        }
        
    }
}