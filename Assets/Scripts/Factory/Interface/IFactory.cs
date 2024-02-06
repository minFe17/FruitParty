using UnityEngine;

public interface IFactory<T>
{
    T MakeObject(Vector2Int position);
}