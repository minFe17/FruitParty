using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SquareBomb : Bomb
{
    protected override void Awake()
    {
        base.Awake();
        _bombType = EBombType.SquareBomb;
    }

    public override void OnEffect()
    {
        _matchFinder.MatchFruits.Union(_matchFinder.GetSquareFruits(_column, _row));
    }
}
