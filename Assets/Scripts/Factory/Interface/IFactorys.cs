using System;
using UnityEngine;

public interface IFactorys
{
    void AddFactorys<TEnum>(TEnum key, IFactory value) where TEnum : Enum;
    object MakeObject<TEnum>(TEnum key, Vector2Int position) where TEnum : Enum;
}