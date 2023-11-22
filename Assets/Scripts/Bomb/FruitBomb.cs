using Utils;

public class FruitBomb : Bomb
{
    FruitManager _fruitManager;
    protected override void Awake()
    {
        base.Awake();
        _bombType = EBombType.FruitBomb;
        _fruitManager = GenericSingleton<FruitManager>.Instance;
    }

    public override void OnEffect()
    {
        if(!_isUse)
        {
            if (_otherFruit != null)
                _matchFinder.MatchFruitOfType(_otherFruit.ColorType);
            else if (_otherFruit == null && _isMatch)
                _matchFinder.MatchFruitOfType(_fruitManager.CurrentFruit.ColorType);
            _isUse = true;
        }
    }
}