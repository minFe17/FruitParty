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

    void AddBombs()
    {
        for (int i = 0; i < (int)EColorType.Max; i++)
            _bombs.Add(Resources.Load($"Prefabs/Bomb/LineBomb/{(EColorType)i}") as GameObject);
    }
}