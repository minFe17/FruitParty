using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SquareBomb : Bomb
{
    StringBuilder _stringBuilder = new StringBuilder();

    protected override void Awake()
    {
        base.Awake();
        _bombType = EBombType.SquareBomb;
        ChangeColor();
    }

    protected override void SetSprite()
    {
        _stringBuilder.Clear();
        string color = _colorType.ToString();
        _stringBuilder.Append(color);
        _stringBuilder.Append("SquareBomb");
        _spriteRenderer.sprite = _spriteManager.BombAtlas.GetSprite(_stringBuilder.ToString());
    }

    protected override void CheckHitTile()
    {
        for (int i = _column - 1; i <= _column + 1; i++)
        {
            for (int j = _row - 1; j <= _row + 1; j++)
            {
                if (i >= 0 && i < _fruitManager.Width && j >= 0 && j < _fruitManager.Height)
                {
                    _bombManager.CheckTile(i, j);
                }
            }
        }
    }

    protected override void GetFruits(out List<Fruit> fruits)
    {
        fruits = new List<Fruit>();
        for (int i = _column - 1; i <= _column + 1; i++)
        {
            for (int j = _row - 1; j <= _row + 1; j++)
            {
                if (i >= 0 && i < _fruitManager.Width && j >= 0 && j < _fruitManager.Height)
                {
                    if (_fruitManager.AllFruits[i, j] != null)
                    {
                        Fruit fruit = _fruitManager.AllFruits[i, j];
                        fruits.Add(fruit);
                        fruit.IsMatch = true;
                    }
                }
            }
        }
    }

    public override void OnEffect()
    {
        if (!_isUse)
        {
            List<Fruit> fruits;
            GetFruits(out fruits);
            _matchFinder.MatchFruits.Union(fruits);
            CheckHitTile();
            _isUse = true;
        }
    }
}