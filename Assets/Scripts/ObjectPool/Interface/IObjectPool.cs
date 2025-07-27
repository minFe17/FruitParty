using System;
using UnityEngine;

public interface IObjectPool
{
    void Init();
    GameObject Pull(Enum type, GameObject prefab);
    void Push(Enum type, GameObject obj);
}