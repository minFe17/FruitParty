using UnityEngine;

public class LavaTileFactory : TileFactoryBase, IFactory<Tile>
{
    protected override void Init()
    {
        _tileType = ETileKindType.Lava;
        _factoryManager.AddFactorys(_tileType, this);
    }

    public Tile MakeObject(Vector2Int position)
    {
        GameObject temp = _objectPoolManager.Pull(_tileType, _prefab);
        temp.transform.position = (Vector2)position;
        return temp.GetComponent<Tile>();
    }
}