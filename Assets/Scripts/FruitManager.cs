using System.Collections.Generic;
using UnityEngine;

public class FruitManager : MonoBehaviour
{
    // ╫л╠шео

    List<GameObject> _fruits = new List<GameObject>();

    public List<GameObject> Fruits
    {
        get
        {
            if (_fruits.Count == 0)
                AddFruit();
            return _fruits;
        }
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
