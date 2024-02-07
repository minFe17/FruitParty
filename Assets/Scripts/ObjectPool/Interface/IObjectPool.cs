using System;
using UnityEngine;

public interface IObjectPool
{
    void Init();
    GameObject Push(Enum type, GameObject prefab);
    void Pull(Enum type, GameObject obj);
}