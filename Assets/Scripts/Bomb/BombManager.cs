using System.Collections.Generic;
using UnityEngine;
using Utils;

public class BombManager : MonoBehaviour
{
    //╫л╠шео
    List<GameObject> _bombs = new List<GameObject>();

    public List<GameObject> Bombs
    {
        get
        {
            if(_bombs.Count == 0)
                AddBombs();
            return _bombs;
        }
    }

    //public void MakeLineBomb(EColorType color)
    //{
    //    Debug.Log(5);
    //    if (_bombs.Count == 0)
    //        AddBombs();
    //    GameObject lineBomb = Instantiate(_bombs[(int)color], transform.position, Quaternion.identity);
    //    GenericSingleton<FruitManager>.Instance.AllFruits[(int)transform.position.x, (int)transform.position.y] = lineBomb.GetComponent<Fruit>();
    //}

    void AddBombs()
    {
        for (int i = 0; i < (int)EColorType.Max; i++)
            _bombs.Add(Resources.Load($"Prefabs/Bomb/LineBomb/{(EColorType)i}") as GameObject);
        Debug.Log(6);
    }
}