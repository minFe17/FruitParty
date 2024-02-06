using UnityEngine;

public interface IFactory<T>
{
    T MakeObject();
}