using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class MatchFinder : MonoBehaviour
{
    // ╫л╠шео
    List<Fruit> _matchFruits = new List<Fruit>();
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
        List<Fruit> fruitsList;
        List<Fruit> bombs;
        BombCount(out fruitsList, out bombs, fruits);

        if (bombs.Count != 0)
            CheckBomb(fruitsList, bombs, fruits, direction);
        else if (fruits[0].FruitType == fruits[1].FruitType && fruits[0].FruitType == fruits[2].FruitType)
            FruitMatch(fruits);
    }

    void CheckBomb(List<Fruit> fruitsList, List<Fruit> bombs, Fruit[] fruits, ELineBombDirectionType direction)
    {
        if (bombs.Count == 3)
        {
            if (bombs[0].ColorType == bombs[1].ColorType && bombs[0].ColorType == bombs[2].ColorType)
            {
                _bombManager.LineBombDirection = direction;
                FruitMatch(fruits);
            }
        }
        else if (bombs.Count == 2)
        {
            if (bombs[0].ColorType == bombs[1].ColorType && bombs[0].ColorType == fruits[0].ColorType)
            {
                _bombManager.LineBombDirection = direction;
                FruitMatch(fruits);
            }
        }
        else if (bombs.Count == 1)
        {
            if (bombs[0].ColorType == fruitsList[0].ColorType && fruitsList[0].FruitType == fruitsList[1].FruitType)
            {
                _bombManager.LineBombDirection = direction;
                FruitMatch(fruits);
            }
        }
    }

    void FruitMatch(Fruit[] fruits)
    {
        AddMatchFruits(fruits[0]);
        AddMatchFruits(fruits[1]);
        AddMatchFruits(fruits[2]);
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

    public void BombCount(out List<Fruit> fruitsList, out List<Fruit> bombs, Fruit[] fruits)
    {
        bombs = new List<Fruit>();
        fruitsList = new List<Fruit>();

        for (int i = 0; i < fruits.Length; i++)
        {
            if (fruits[i].IsBomb)
                bombs.Add(fruits[i]);
        }
        fruitsList = fruits.Where(child => child != child.IsBomb).ToList();
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