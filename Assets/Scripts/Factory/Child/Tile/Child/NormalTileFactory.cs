using UnityEngine;

public class NormalTileFactory : TileFactoryBase, IFactory<Tile>
{
    protected override void Init()
    {
        _tileType = ETileKindType.Normal;
        _factoryManager.AddFactorys(_tileType, this);
    }

    public Tile MakeObject(Vector2Int position) 
    {
        GameObject temp = _objectPoolManager.Push(_tileType, _prefab);
        temp.transform.position = (Vector2)position;
        return temp.GetComponent<Tile>();
    }
}