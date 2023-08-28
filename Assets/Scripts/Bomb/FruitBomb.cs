public class FruitBomb : Bomb
{
    protected override void Awake()
    {
        base.Awake();
        _bombType = EBombType.FruitBomb;
    }

    public override void OnEffect()
    {
        _matchFinder.MatchFruitOfType(_otherFruit.ColorType);
    }
}
