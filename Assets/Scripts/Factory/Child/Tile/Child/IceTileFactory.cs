using UnityEngine;

public class IceTileFactory : TileFactoryBase, IFactory<Tile>
{
    protected override void Init()
    {
        _tileType = ETileKindType.Ice;
        _factoryManager.AddFactorys(_tileType, this);
    }

    public Tile MakeObject(Vector2Int position)
    {
        GameObject temp = _tileObjectPool.Push(_tileType, _prefab);
        temp.transform.position = (Vector2)position;
        return temp.GetComponent<Tile>();
    }
}