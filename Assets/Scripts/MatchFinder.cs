using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class MatchFinder : MonoBehaviour
{
    // ╫л╠шео
    List<Fruit> _matchFruits = new List<Fruit>();
    List<Fruit> _fruitsList = new List<Fruit>();
    List<Fruit> _bombs = new List<Fruit>();

    FruitManager _fruitManager;
    BombManager _bombManager;

    public List<Fruit> MatchFruits { get => _matchFruits; }

    void Start()
    {
        _fruitManager = GenericSingleton<FruitManager>.Instance;
        _bombManager = GenericSingleton<BombManager>.Instance;
    }

    void FindFruitMatch(Fruit[] fruits, ELineBombDirectionType direction)
    {
        _fruitsList.Clear();
        _bombs.Clear();
        BombCount(fruits);

        if (_bombs.Count != 0)
            CheckBomb(fruits, direction);
        else if (fruits[0].FruitType == fruits[1].FruitType && fruits[0].FruitType == fruits[2].FruitType)
            FruitMatch(fruits);
    }

    void CheckBomb(Fruit[] fruits, ELineBombDirectionType direction)
    {
        if (_bombs.Count == 3)
        {
            if (_bombs[0].ColorType == _bombs[1].ColorType && _bombs[0].ColorType == _bombs[2].ColorType)
            {
                _bombManager.LineBombDirection = direction;
                FruitMatch(fruits);
            }
        }
        else if (_bombs.Count == 2)
        {
            if (_bombs[0].ColorType == _bombs[1].ColorType && _bombs[0].ColorType == fruits[0].ColorType)
            {
                _bombManager.LineBombDirection = direction;
                FruitMatch(fruits);
            }
        }
        else if (_bombs.Count == 1)
        {
            if (_bombs[0].ColorType == _fruitsList[0].ColorType && _fruitsList[0].FruitType == _fruitsList[1].FruitType)
            {
                _bombManager.LineBombDirection = direction;
                FruitMatch(fruits);
            }
        }
    }

    void FruitMatch(Fruit[] fruits)
    {
        for (int i = 0; i < fruits.Length; i++)
            AddMatchFruits(fruits[i]);
    }

    void AddMatchFruits(Fruit fruit)
    {
        if (!_matchFruits.Contains(fruit))
            _matchFruits.Add(fruit);
        if (!fruit.IsMatch)
            fruit.IsMatch = true;
    }

    public void MatchFruitOfType(EColorType color)
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

    void BombCount(Fruit[] fruits)
    {
        for (int i = 0; i < fruits.Length; i++)
        {
            if (fruits[i].IsBomb)
                _bombs.Add(fruits[i]);
        }
        _fruitsList = fruits.Where(child => child != child.IsBomb).ToList();
    }

    bool CheckBombMatch()
    {
        if (_bombs.Count == 3)
        {
            if (_bombs[0].ColorType == _bombs[1].ColorType && _bombs[0].ColorType == _bombs[2].ColorType)
                return true;
        }
        else if (_bombs.Count == 2)
        {
            if (_bombs[0].ColorType == _bombs[1].ColorType && _bombs[0].ColorType == _fruitsList[0].ColorType)
                return true;
        }
        else if (_bombs.Count == 1)
        {
            if (_bombs[0].ColorType == _fruitsList[0].ColorType && _fruitsList[0].FruitType == _fruitsList[1].FruitType)
                return true;
        }
        return false;
    }

    void ClearList()
    {
        _fruitsList.Clear();
        _bombs.Clear();
    }

    public bool CheckMatch(Fruit[] fruits)
    {
        BombCount(fruits);
        Vector2Int firstPos = new Vector2Int(fruits[0].Column, fruits[0].Row);
        Vector2Int secondPos = new Vector2Int(fruits[1].Column, fruits[1].Row);
        Vector2Int thirdPos = new Vector2Int(fruits[2].Column, fruits[2].Row);

        if (_bombs.Count != 0)
        {
            if (CheckBombMatch())
            {
                ClearList();
                return true;
            }
        }
        else if (_fruitManager.AllFruits[firstPos.x, firstPos.y].FruitType == _fruitManager.AllFruits[secondPos.x, secondPos.y].FruitType
              && _fruitManager.AllFruits[firstPos.x, firstPos.y].FruitType == _fruitManager.AllFruits[thirdPos.x, thirdPos.y].FruitType)
        {
            ClearList();
            return true;
        }

        ClearList();
        return false;
    }

    public void FindAllMatch()
    {
        StartCoroutine(FindAllMatchRoutine());
    }

    IEnumerator FindAllMatchRoutine()
    {
        Fruit[] fruits = new Fruit[3];
        yield return null;

        for (int i = 0; i < _fruitManager.Width; i++)
        {
            for (int j = 0; j < _fruitManager.Height; j++)
            {
                fruits[0] = _fruitManager.AllFruits[i, j];
                if (fruits[0] != null)
                {
                    if (i > 0 && i < _fruitManager.Width - 1)
                    {
                        fruits[1] = _fruitManager.AllFruits[i - 1, j];
                        fruits[2] = _fruitManager.AllFruits[i + 1, j];
                        if (fruits[1] != null && fruits[2] != null)
                            FindFruitMatch(fruits, ELineBombDirectionType.Column);
                    }
                    if (j > 0 && j < _fruitManager.Height - 1)
                    {
                        fruits[1] = _fruitManager.AllFruits[i, j + 1];
                        fruits[2] = _fruitManager.AllFruits[i, j - 1];
                        if (fruits[1] != null && fruits[2] != null)
                            FindFruitMatch(fruits, ELineBombDirectionType.Row);
                    }
                }
            }
        }
    }
}