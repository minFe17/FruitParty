using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitManager : MonoBehaviour
{
    // ╫л╠шео
    Fruit[,] _allFruits;
    List<GameObject> _fruits = new List<GameObject>();

    int _width;
    int _height;

    public Fruit[,] AllFruits { get => _allFruits; }
    public int Width { get => _width; }
    public int Height { get => _height; }
    public int Offset { get; set; }

    public void Init(int x, int y)
    {
        _width = x;
        _height = y;
        _allFruits = new Fruit[x, y];
    }

    public void CreateFruit(Transform parent, Vector2 position)
    {
        if (_fruits.Count == 0)
            AddFruit();
        int fruitNumber = 0;
        int iterations = 0;
        int x = (int)position.x;
        int y = (int)position.y - Offset;
        do
        {
            fruitNumber = Random.Range(0, _fruits.Count);
            iterations++;
        }
        while (MatchAt(x, y, fruitNumber) && iterations <= 100);

        GameObject fruit = Instantiate(_fruits[fruitNumber], position, Quaternion.identity);
        fruit.GetComponent<Fruit>().Column = x;
        fruit.GetComponent<Fruit>().Row = y;
        fruit.transform.parent = parent;
        _allFruits[x, y] = fruit.GetComponent<Fruit>();
    }

    public void CheckMatchsFruit()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_allFruits[i, j] != null)
                    DestroyMatchFruit(i, j);
            }
        }
        StartCoroutine(DecreaseRowRoutine());
    }

    void AddFruit()
    {
        for (int i = 0; i < (int)EFruitType.Max; i++)
            _fruits.Add(Resources.Load($"Prefabs/Fruits/{(EFruitType)i}") as GameObject);
    }

    bool MatchAt(int column, int row, int fruitNumber)
    {
        Fruit fruit = _fruits[fruitNumber].GetComponent<Fruit>();

        if (column > 1 && row > 1)
        {
            if (_allFruits[column - 1, row].FruitType == fruit.FruitType && _allFruits[column - 2, row].FruitType == fruit.FruitType)
                return true;
            if (_allFruits[column, row - 1].FruitType == fruit.FruitType && _allFruits[column, row - 2].FruitType == fruit.FruitType)
                return true;
        }
        else if (column <= 1 || row <= 1)
        {
            if (column > 1)
            {
                if (_allFruits[column - 1, row].FruitType == fruit.FruitType && _allFruits[column - 2, row].FruitType == fruit.FruitType)
                    return true;
            }
            if (row > 1)
            {
                if (_allFruits[column, row - 1].FruitType == fruit.FruitType && _allFruits[column, row - 2].FruitType == fruit.FruitType)
                    return true;
            }
        }
        return false;
    }

    void DestroyMatchFruit(int column, int row)
    {
        if (_allFruits[column, row].IsMatch)
        {
            Destroy(_allFruits[column, row].gameObject);
            _allFruits[column, row] = null;
        }
    }

    void RefillFruit()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_allFruits[i, j] == null)
                {
                    Vector2 position = new Vector2(i, j + Offset);
                    int fruitNumber = Random.Range(0, _fruits.Count);
                    GameObject fruit = Instantiate(_fruits[fruitNumber], position, Quaternion.identity);
                    _allFruits[i, j] = fruit.GetComponent<Fruit>();
                    fruit.GetComponent<Fruit>().Column = i;
                    fruit.GetComponent<Fruit>().Row = j;
                }
            }
        }
    }

    bool MatchOnboard()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_allFruits[i, j] != null)
                {
                    if (_allFruits[i, j].IsMatch)
                        return true;
                }
            }
        }
        return false;
    }

    IEnumerator DecreaseRowRoutine()
    {
        int nullCount = 0;
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_allFruits[i, j] == null)
                    nullCount++;
                else if (nullCount > 0)
                {
                    _allFruits[i, j].Row -= nullCount;
                    _allFruits[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(FillFruitRoutine());
    }

    IEnumerator FillFruitRoutine()
    {
        RefillFruit();
        yield return new WaitForSeconds(0.5f);

        while (MatchOnboard())
        {
            yield return new WaitForSeconds(0.5f);
            CheckMatchsFruit();
        }
    }
}

public enum EFruitType
{
    Carrot,
    Lemon,
    Orange,
    StarFruit,
    Strawberry,
    Tomato,
    Watermelon,
    Max,
}
