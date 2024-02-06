using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class FactoryManager<TEnum, T> : MonoBehaviour where TEnum : Enum
{
    // ╫л╠шео
    Dictionary<TEnum, IFactory<T>> _factorys = new Dictionary<TEnum, IFactory<T>>();

    GameObject _factoryPrefab;
    AddressableManager _addressableManager;

    public int Count { get => _factorys.Count; }

    public async void Init()
    {
        if (_factoryPrefab == null)
        {
            if (_addressableManager == null)
                _addressableManager = GenericSingleton<AddressableManager>.Instance;
            _factoryPrefab = await _addressableManager.GetAddressableAsset<GameObject>("Factory");
        }
        CreateFactory();
    }

    void CreateFactory()
    {
        Instantiate(_factoryPrefab, transform);
    }

    public void AddFactorys(TEnum key, IFactory<T> value)
    {
        _factorys.Add(key, value);
    }

    public T MakeObject(TEnum key)
    {
        IFactory<T> factory;
        _factorys.TryGetValue(key, out factory);
        return factory.MakeObject();
    }
}