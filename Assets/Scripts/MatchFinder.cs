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
        ClearList();
        BombCount(fruits);

        if (_bombs.Count != 0)
            CheckBomb(fruits, direction);
        else if (fruits[0].FruitType == fruits[1].FruitType && fruits[0].FruitType == fruits[2].FruitType)
            FruitMatch(fruits);

        ClearList();
    }

    void CheckBomb(Fruit[] fruits, ELineBombDirectionType direction)
    {
        if (CheckBombMatch())
        {
            _bombManager.LineBombDirection = direction;
            FruitMatch(fruits);
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
            return (_bombs[0].ColorType == _bombs[1].ColorType && _bombs[0].ColorType == _bombs[2].ColorType);
        }
        else if (_bombs.Count == 2)
        {
            return (_bombs[0].ColorType == _bombs[1].ColorType && _bombs[0].ColorType == _fruitsList[0].ColorType);
        }
        else if (_bombs.Count == 1)
        {
            return (_bombs[0].ColorType == _fruitsList[0].ColorType && _fruitsList[0].FruitType == _fruitsList[1].FruitType);
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

        if (_bombs.Count != 0)
        {
            if (CheckBombMatch())
            {
                ClearList();
                return true;
            }
        }
        else if (fruits[0].FruitType == fruits[1].FruitType && fruits[0].FruitType == fruits[2].FruitType)
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