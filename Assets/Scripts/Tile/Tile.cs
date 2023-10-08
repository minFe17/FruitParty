using UnityEngine;
using Utils;

public abstract class Tile : MonoBehaviour
{
    protected TileManager _tileManager;
    protected TileType _tileType;
    protected int _x;
    protected int _y;

    public TileType TileType { get => _tileType; }

    public virtual void Init(TileType tileType, int x, int y)
    {
        _tileManager = GenericSingleton<TileManager>.Instance;
        _tileType = tileType;
        _x = x;
        _y = y;

    }

    public virtual void TakeDamage()
    {

    }

    public abstract void DestroyTile();
}
