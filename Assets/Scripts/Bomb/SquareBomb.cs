using System.Linq;

public class SquareBomb : Bomb
{
    protected override void Awake()
    {
        base.Awake();
        _bombType = EBombType.SquareBomb;
    }

    public override void OnEffect()
    {
        if (!_isUse)
        {
            _matchFinder.MatchFruits.Union(_bombManager.GetSquareFruits(_column, _row));
            _bombManager.HitTileSquareBomb(_column, _row);
            _isUse = true;
        }
    }
}