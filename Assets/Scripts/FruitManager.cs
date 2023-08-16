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

    public void Init(int x, int y)
    {
        _width = x;
        _height = y;
        _allFruits = new Fruit[x, y];
    }

    public void CreateFruit(Transform parent, Vector2Int position)
    {
        if (_fruits.Count == 0)
            AddFruit();
        int fruitNumber = 0;
        int iterations = 0;
        do
        {
            fruitNumber = Random.Range(0, _fruits.Count);
            iterations++;
        }
        while (MatchAt(position.x, position.y, fruitNumber) && iterations <= 100);

        GameObject fruit = Instantiate(_fruits[fruitNumber], parent.position, Quaternion.identity);
        fruit.transform.parent = parent;
        _allFruits[position.x, position.y] = fruit.GetComponent<Fruit>();
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
