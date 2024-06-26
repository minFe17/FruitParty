using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LineBomb : Bomb
{
    StringBuilder _stringBuilder = new StringBuilder();
    ELineBombDirectionType _lineDirection;

    protected override void Awake()
    {
        base.Awake();
        _bombType = EBombType.LineBomb;
        ChangeColor();
    }

    protected override void SetSprite()
    {
        _stringBuilder.Clear();
        string color = _colorType.ToString();
        _stringBuilder.Append(color);
        _stringBuilder.Append("LineBomb");
        _spriteRenderer.sprite = _spriteManager.BombAtlas.GetSprite(_stringBuilder.ToString());
    }

    protected override void CheckHitTile()
    {
        if (_lineDirection == ELineBombDirectionType.Column)
            HitTileColumn();
        else if (_lineDirection == ELineBombDirectionType.Row)
            HitTileRow();
    }

    protected override void GetFruits(out List<Fruit> fruits)
    {
        fruits = new List<Fruit>();
        if (_lineDirection == ELineBombDirectionType.Column)
            GetColumnFruit(out fruits);
        else if (_lineDirection == ELineBombDirectionType.Row)
            GetRowFruit(out fruits);
    }

    public override void OnEffect()
    {
        _lineDirection = _bombManager.LineBombDirection;
        List<Fruit> fruits;
        if (!_isUse)
        {
            if (_lineDirection == ELineBombDirectionType.None || _lineDirection == ELineBombDirectionType.Max)
                _lineDirection = (ELineBombDirectionType)Random.Range(1, (int)ELineBombDirectionType.Max);

            GetFruits(out fruits);
            _matchFinder.MatchFruits.Union(fruits);
            CheckHitTile();

            _bombManager.LineBombDirection = ELineBombDirectionType.None;
            _isUse = true;
        }
    }

    void GetColumnFruit(out List<Fruit> fruits)
    {
        fruits = new List<Fruit>();
        for (int i = 0; i < _fruitManager.Height; i++)
        {
            if (_fruitManager.AllFruits[_column, i] != null)
            {
                Fruit fruit = _fruitManager.AllFruits[_column, i];
                if (fruit.IsBomb && fruit.BombType == EBombType.LineBomb)
                    _bombManager.ChangeLineDirection();

                fruits.Add(fruit);
                fruit.IsMatch = true;
            }
        }
    }

    void GetRowFruit(out List<Fruit> fruits)
    {
        fruits = new List<Fruit>();
        for (int i = 0; i < _fruitManager.Width; i++)
        {
            if (_fruitManager.AllFruits[i, _row] != null)
            {
                Fruit fruit = _fruitManager.AllFruits[i, _row];
                if (fruit.IsBomb && fruit.BombType == EBombType.LineBomb)
                    _bombManager.ChangeLineDirection();

                fruits.Add(fruit);
                fruit.IsMatch = true;
            }
        }
    }

    void HitTileColumn()
    {
        for (int i = 0; i < _fruitManager.Height; i++)
            _bombManager.CheckTile(_column, i);
    }

    void HitTileRow()
    {
        for (int i = 0; i < _fruitManager.Width; i++)
        {
            _bombManager.CheckTile(i, _row);
        }
    }
}