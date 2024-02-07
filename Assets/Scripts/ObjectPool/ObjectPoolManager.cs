using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    // ╫л╠шео
    Dictionary<Type, IObjectPool> _objectPools = new Dictionary<Type, IObjectPool>();

    void Awake()
    {
        _objectPools.Add(typeof(EFruitType), new ObjectPool<EFruitType>());
        _objectPools.Add(typeof(EBombType), new ObjectPool<EBombType>());
        _objectPools.Add(typeof(ETileKindType), new ObjectPool<ETileKindType>());
        _objectPools.Add(typeof(EEffectType), new ObjectPool<EEffectType>());

        CreateQueue();
    }

    void CreateQueue()
    {
        _objectPools[typeof(EFruitType)].Init();
        _objectPools[typeof(EBombType)].Init();
        _objectPools[typeof(ETileKindType)].Init();
        _objectPools[typeof(EEffectType)].Init();
    }

    public GameObject Push<TEnum>(TEnum type, GameObject prefab) where TEnum : Enum
    {
        IObjectPool objectPool;
        _objectPools.TryGetValue(typeof(TEnum), out objectPool);
        return objectPool.Push(type, prefab);
    }

    public void Pull<TEnum>(TEnum type, GameObject obj) where TEnum : Enum
    {
        IObjectPool objectPool;
        _objectPools.TryGetValue(typeof(TEnum), out objectPool);
        objectPool.Pull(type, obj);
    }
}
