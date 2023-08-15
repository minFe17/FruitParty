using System.Collections.Generic;
using UnityEngine;

public class FruitManager : MonoBehaviour
{
    // ╫л╠шео
    GameObject[,] _allFruits;
    List<GameObject> _fruits = new List<GameObject>();

    Board _board;

    int _width;
    int _height;

    public GameObject[,] AllFruits { get => _allFruits; }
    public Board Board { get => _board;}
    public int Width { get => _width; }
    public int Height { get => _height; }

    public void Init(int x, int y)
    {
        _width = x;
        _height = y;
        _allFruits = new GameObject[x, y];
    }

    public void CreateFruit(Transform parent, Vector2Int position)
    {
        if (_fruits.Count == 0)
            AddFruit();
        int fruitNumber = Random.Range(0, _fruits.Count);
        GameObject fruit = Instantiate(_fruits[fruitNumber], parent.position, Quaternion.identity);
        fruit.transform.parent = parent;
        _allFruits[position.x, position.y] = fruit;
    }

    void AddFruit()
    {
        for (int i = 0; i < (int)EFruitType.Max; i++)
            _fruits.Add(Resources.Load($"Prefabs/Fruits/{(EFruitType)i}") as GameObject);
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
