using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<TEnum> : MonoBehaviour where TEnum : Enum
{
    // ╫л╠шео
    Dictionary<TEnum, Queue<GameObject>> _objectPool = new Dictionary<TEnum, Queue<GameObject>>();
    Queue<GameObject> _queue;

    TEnum[] _enumValue;

    public void Init()
    {
        _enumValue = (TEnum[])Enum.GetValues(typeof(TEnum));

        for (int i = 0; i < _enumValue.Length; i++)
            _objectPool.Add(_enumValue[i], new Queue<GameObject>());
    }

    GameObject CreateObject(GameObject prefab)
    {
        return Instantiate(prefab);
    }

    public GameObject Push(TEnum type, GameObject prefab)
    {
        _queue = null;
        GameObject returnObject = null;
        _objectPool.TryGetValue(type, out _queue);
        if (_queue.Count > 0)
            returnObject = _queue.Dequeue();
        else
            returnObject = CreateObject(prefab);
        return returnObject;
    }

    public void Pull(TEnum type, GameObject target)
    {
        _queue = null;
        _objectPool.TryGetValue(type, out _queue);
        target.SetActive(false);
        _queue.Enqueue(gameObject);
        target.transform.parent = this.transform;
    }
}