using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Tile : MonoBehaviour
{
    List<GameObject> _fruits;

    void Start()
    {
        _fruits = GenericSingleton<FruitManager>.Instance.Fruits;
        Init();
    }

    void Init()
    {
        int fruitNumber = Random.Range(0, _fruits.Count);
        GameObject fruit = Instantiate(_fruits[fruitNumber], transform.position, Quaternion.identity);
        fruit.transform.parent = this.transform;

    }
}
