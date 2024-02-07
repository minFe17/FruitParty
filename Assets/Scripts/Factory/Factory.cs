using System;
using System.Collections.Generic;
using UnityEngine;

public class Factory<T> : IFactorys
{
    Dictionary<Enum, IFactory<T>> _factorys = new Dictionary<Enum, IFactory<T>>();

    public Dictionary<Enum, IFactory<T>> Factorys { get => _factorys; }

    void IFactorys.AddFactorys<TEnum>(TEnum key, IFactory value)
    {
        _factorys.Add(key, (IFactory<T>)value);
    }

    object IFactorys.MakeObject<TEnum>(TEnum key, Vector2Int position)
    {
        IFactory<T> factory;
        _factorys.TryGetValue(key, out factory);
        return factory.MakeObject(position);
    }
}