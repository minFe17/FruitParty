using System;
using System.Collections.Generic;
using UnityEngine;

public class BombManager : MonoBehaviour
{
    //╫л╠шео
    List<GameObject> _lineBombs = new List<GameObject>();
    List<GameObject> _squareBombs = new List<GameObject>();

    public List<GameObject> LineBombs
    {
        get
        {
            if(_lineBombs.Count == 0)
                AddLineBombs();
            return _lineBombs;
        }
    }

    public List<GameObject> SquareBombs
    {
        get
        {
            if (_squareBombs.Count == 0)
                AddSquareBombs();
            return _squareBombs;
        }
    }

    void AddLineBombs()
    {
        for (int i = 0; i < (int)EColorType.Max; i++)
            _lineBombs.Add(Resources.Load($"Prefabs/Bomb/LineBomb/{(EColorType)i}") as GameObject);
    }

    void AddSquareBombs()
    {
        for (int i = 0; i < (int)EColorType.Max; i++)
            _lineBombs.Add(Resources.Load($"Prefabs/Bomb/SquareBomb/{(EColorType)i}") as GameObject);
    }
}