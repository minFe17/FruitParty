using System.Collections.Generic;

public class FruitBomb : Bomb
{
    protected override void Awake()
    {
        base.Awake();
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
            GetFruits();
            _isUse = true;
        }
    }

    void GetFruits()
    {
        if (_otherFruit != null)
            MatchFruitOfType(_otherFruit.ColorType);
        else if (_otherFruit == null && _isMatch)
            MatchFruitOfType(_fruitManager.CurrentFruit.ColorType);
    }

    void MatchFruitOfType(EColorType color)
    {
        for (int i = 0; i < _fruitManager.Width; i++)
        {
            for (int j = 0; j < _fruitManager.Height; j++)
            {
                if (_fruitManager.AllFruits[i, j] != null)
                {
                    if (_fruitManager.AllFruits[i, j].ColorType == color)
                        _fruitManager.AllFruits[i, j].IsMatch = true;
                }
            }
        }
    }
}