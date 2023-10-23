using System.Collections.Generic;
using UnityEngine;
using Utils;

public class BombManager : MonoBehaviour
{
    // ╫л╠шео
    List<GameObject> _lineBombs = new List<GameObject>();
    List<GameObject> _squareBombs = new List<GameObject>();

    GameObject _fruitBomb;
    ELineBombDirectionType _lineBombDirection;
    FruitManager _fruitManager;
    TileManager _tileManager;

    void Start()
    {
        _fruitManager = GenericSingleton<FruitManager>.Instance;
        _tileManager = GenericSingleton<TileManager>.Instance;
    }

    public List<GameObject> LineBombs
    {
        get
        {
            if (_lineBombs.Count == 0)
                AddLineBombs();
            return _lineBombs;
        }
    }

    public List<GameObject> SquareBombs
    {
        get
        {
            if (_squareBombs.Count == 0)
                AddSquareBombs();
            return _squareBombs;
        }
    }

    public GameObject FruitBomb
    {
        get
        {
            if (_fruitBomb == null)
                _fruitBomb = Resources.Load("Prefabs/Bomb/FruitBomb") as GameObject;
            return _fruitBomb;
        }
    }

    public ELineBombDirectionType LineBombDirection { get => _lineBombDirection; set => _lineBombDirection = value; }


    void AddLineBombs()
    {
        for (int i = 0; i < (int)EColorType.Max; i++)
            _lineBombs.Add(Resources.Load($"Prefabs/Bomb/LineBomb/{(EColorType)i}") as GameObject);
    }

    void AddSquareBombs()
    {
        for (int i = 0; i < (int)EColorType.Max; i++)
            _squareBombs.Add(Resources.Load($"Prefabs/Bomb/SquareBomb/{(EColorType)i}") as GameObject);
    }

    void CheckConcreteTile(int column, int row)
    {
        if (_tileManager.ConcreteTiles[column, row])
        {
            _tileManager.DestroyTile(_tileManager.ConcreteTiles[column, row]);
        }
    }

    void CheckLavaTile(int column, int row)
    {
        if (_tileManager.LavaTiles[column, row])
        {
            _tileManager.DestroyTile(_tileManager.LavaTiles[column, row]);
            _tileManager.CreateMoreLavaTile = false;
        }
    }

    public List<Fruit> GetColumnFruits(int column)
    {
        List<Fruit> fruits = new List<Fruit>();
        for (int i = 0; i < _fruitManager.Height; i++)
        {
            if (_fruitManager.AllFruits[column, i] != null)
            {
                Fruit fruit = _fruitManager.AllFruits[column, i];
                if (fruit.IsBomb && fruit.BombType == EBombType.LineBomb)
                    _lineBombDirection = ELineBombDirectionType.Row;

                fruits.Add(fruit);
                fruit.IsMatch = true;
            }
        }
        return fruits;
    }

    public List<Fruit> GetRowFruits(int row)
    {
        List<Fruit> fruits = new List<Fruit>();
        for (int i = 0; i < _fruitManager.Width; i++)
        {
            if (_fruitManager.AllFruits[i, row] != null)
            {
                Fruit fruit = _fruitManager.AllFruits[i, row];
                if (fruit.IsBomb && fruit.BombType == EBombType.LineBomb)
                    _lineBombDirection = ELineBombDirectionType.Column;

                fruits.Add(fruit);
                fruit.IsMatch = true;
            }
        }
        return fruits;
    }

    public List<Fruit> GetSquareFruits(int column, int row)
    {
        List<Fruit> fruits = new List<Fruit>();
        for (int i = column - 1; i <= column + 1; i++)
        {
            for (int j = row - 1; j <= row + 1; j++)
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
        return fruits;
    }

    public void HitTileColumnLineBomb(int column)
    {
        for (int i = 0; i < _fruitManager.Height; i++)
        {
            CheckConcreteTile(column, i);
            CheckLavaTile(column, i);
        }
    }

    public void HitTileRowLineBomb(int row)
    {
        for (int i = 0; i < _fruitManager.Width; i++)
        {
            CheckConcreteTile(i, row);
            CheckLavaTile(i, row);
        }
    }

    public void HitTileSquareBomb(int column, int row)
    {
        for (int i = column - 1; i <= column + 1; i++)
        {
            for (int j = row - 1; j <= row + 1; j++)
            {
                if (i >= 0 && i < _fruitManager.Width && j >= 0 && j < _fruitManager.Height)
                {
                    CheckConcreteTile(i, j);
                    CheckLavaTile(i, j);
                }
            }
        }
    }
}