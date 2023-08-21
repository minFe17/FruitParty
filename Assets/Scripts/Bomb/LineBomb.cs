using System.Linq;
using Utils;

public class LineBomb : Bomb
{
    protected override void Awake()
    {
        base.Awake();
        _bombType = EBombType.LineBomb;
    }

    public override void OnEffect()
    {
        MatchFinder matchFinder = GenericSingleton<MatchFinder>.Instance;

        if (matchFinder.LineBombDirection == ELineBombDirectionType.Column)
            matchFinder.MatchFruits.Union(matchFinder.GetColumnFruits(_column));
        if(matchFinder.LineBombDirection == ELineBombDirectionType.Row)
            matchFinder.MatchFruits.Union(matchFinder.GetRowFruits(_row));
        
        matchFinder.LineBombDirection = ELineBombDirectionType.None;
    }
}
