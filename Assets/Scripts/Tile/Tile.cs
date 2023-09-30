using UnityEngine;
using Utils;

public class Tile : MonoBehaviour
{
    protected TileManager _tileManager;
    protected int _x;
    protected int _y;

    public virtual void Init(int x, int y)
    {
        _tileManager = GenericSingleton<TileManager>.Instance;
        _x = x;
        _y = y;
    }
}
