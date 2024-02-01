public class FruitBomb : Bomb
{
    protected override void Start()
    {
        base.Start();
        _bombType = EBombType.FruitBomb;
    }

    protected override void SetSprite()
    {
        _spriteRenderer.sprite = _spriteManager.BombAtlas.GetSprite("FruitBomb");
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