using System.Collections.Generic;
using UnityEngine;
using Utils;

public class BombManager : MonoBehaviour
{
    // ╫л╠шео
    FactoryManager _factoryManager;
    FruitManager _fruitManager;
    TileManager _tileManager;
    MatchFinder _matchFinder;
    ELineBombDirectionType _lineBombDirection;

    public ELineBombDirectionType LineBombDirection { get => _lineBombDirection; set => _lineBombDirection = value; }

    public void Init()
    {
        _factoryManager = GenericSingleton<FactoryManager>.Instance;
        _fruitManager = GenericSingleton<FruitManager>.Instance;
        _tileManager = GenericSingleton<TileManager>.Instance;
        _matchFinder = GenericSingleton<MatchFinder>.Instance;
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

    public void CreateBomb(EBombType bombType, Fruit fruit)
    {
        Vector2Int position = new Vector2Int(fruit.Column, fruit.Row);
        _factoryManager.ColorType = fruit.ColorType;
        Debug.Log(fruit.ColorType);
        Bomb bomb = (Bomb)_factoryManager.MakeObject<EBombType, Bomb>(bombType, position);
        bomb.transform.position = fruit.transform.position;

        _fruitManager.DestroyFruit(fruit);
        _fruitManager.AllFruits[fruit.Column, fruit.Row] = bomb;
        _matchFinder.MatchFruits.Clear();
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