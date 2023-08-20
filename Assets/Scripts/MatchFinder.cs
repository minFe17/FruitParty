using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class MatchFinder : MonoBehaviour
{
    //╫л╠шео
    List<Fruit> _matchFruits = new List<Fruit>();
    ELineBombDirectionType _lineBombDirection;
    FruitManager _fruitManager;
    BombManager _bombManager;

    public List<Fruit> MatchFruits { get => _matchFruits; }
    public ELineBombDirectionType LineBombDirection { get => _lineBombDirection; }

    void Start()
    {
        _fruitManager = GenericSingleton<FruitManager>.Instance;
        _bombManager = GenericSingleton<BombManager>.Instance;
    }

    public void FindAllMatch()
    {
        StartCoroutine(FindAllMatchRoutine());
    }

    public void CheckLineBomb()
    {
        if (_fruitManager.CurrentFruit != null)
        {
            Debug.Log(3);
            if (_fruitManager.CurrentFruit.IsMatch)
            {
                Debug.Log(4);
                _fruitManager.CurrentFruit.MakeLineBomb();

            }
        }
        else if (_fruitManager.CurrentFruit.OtherFruit != null)
        {
            Debug.Log(4);
            Fruit otherFruit = _fruitManager.CurrentFruit.OtherFruit;
            if(otherFruit.IsMatch)
            {
                Debug.Log(4.5f);
                otherFruit.MakeLineBomb();

            }
        }
    }

    public List<Fruit> GetColumnFruits(int column)
    {
        List<Fruit> fruits = new List<Fruit>();
        for (int i = 0; i < _fruitManager.Height; i++)
        {
            if (_fruitManager.AllFruits[column, i] != null)
            {
                fruits.Add(_fruitManager.AllFruits[column, i]);
                _fruitManager.AllFruits[column, i].IsMatch = true;
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
                fruits.Add(_fruitManager.AllFruits[i, row]);
                _fruitManager.AllFruits[i, row].IsMatch = true;
            }
        }
        return fruits;
    }

    void FindFruitMatch(Fruit[] fruits)
    {
        List<Fruit> fruitsList;
        List<Fruit> bombs;
        BombCount(out fruitsList, out bombs, fruits);

        if (bombs.Count != 0)
            CheckBomb(fruitsList, bombs, fruits);
        else if (fruits[0].FruitType == fruits[1].FruitType && fruits[0].FruitType == fruits[2].FruitType)
            FruitMatch(fruits);
    }

    void CheckBomb(List<Fruit> fruitsList, List<Fruit> bombs, Fruit[] fruits)
    {
        if(bombs.Count == 1)
        {
            if (bombs[0].ColorType == fruitsList[0].ColorType && fruitsList[0].FruitType == fruitsList[1].FruitType)
                FruitMatch(fruits);
        }
        if(bombs.Count == 2)
        {
            if (bombs[0].ColorType == bombs[1].ColorType && bombs[0].ColorType == fruits[0].ColorType)
                FruitMatch(fruits);
        }
        if(bombs.Count == 3)
        {
            if (bombs[0].ColorType == bombs[1].ColorType && bombs[0].ColorType == bombs[2].ColorType)
                FruitMatch(fruits);
        }
    }

    void BombCount(out List<Fruit> fruitsList, out List<Fruit> bombs, Fruit[] fruits)
    {
        bombs = new List<Fruit>();

        for(int i=0; i<fruits.Length;i++)
        {
            if (fruits[i].IsBomb)
            {
                bombs.Add(fruits[i]);
                fruits[i] = null;
            }
        }

        fruitsList = fruits.Where(child => child != null).ToList();
    }

    void FruitMatch(Fruit[] fruits)
    {
        if (!_matchFruits.Contains(fruits[0]))
            _matchFruits.Add(fruits[0]);
        if (!_matchFruits.Contains(fruits[1]))
            _matchFruits.Add(fruits[1]);
        if (!_matchFruits.Contains(fruits[2]))
            _matchFruits.Add(fruits[2]);

        fruits[0].IsMatch = true;
        fruits[1].IsMatch = true;
        fruits[2].IsMatch = true;
    }

    IEnumerator FindAllMatchRoutine()
    {
        Fruit[] fruits = new Fruit[3];

        yield return new WaitForSeconds(0.2f);
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
                        {
                            _lineBombDirection = ELineBombDirectionType.Column;
                            FindFruitMatch(fruits);
                        }

                    }
                    if (j > 0 && j < _fruitManager.Height - 1)
                    {
                        fruits[1] = _fruitManager.AllFruits[i, j + 1];
                        fruits[2] = _fruitManager.AllFruits[i, j - 1];
                        if (fruits[1] != null && fruits[2] != null)
                        {
                            _lineBombDirection = ELineBombDirectionType.Row;
                            FindFruitMatch(fruits);
                        }
                    }
                }
            }
        }
    }
}