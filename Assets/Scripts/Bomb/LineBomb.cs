using System.Linq;
using Utils;

public class LineBomb : Bomb
{
    void Awake()
    {
        _bombType = EBombType.LineBomb;
    }

    public override void OnEffect()
    {

        MatchFinder matchFinder = GenericSingleton<MatchFinder>.Instance;
        if (matchFinder.LineBombDirection == ELineBombDirectionType.Column)
            matchFinder.MatchFruits.Union(matchFinder.GetColumnFruits(_row));
        else
            matchFinder.MatchFruits.Union(matchFinder.GetRowFruits(_column));
    }
}
