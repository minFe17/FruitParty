using UnityEngine;
using UnityEngine.U2D;
using Utils;

public abstract class Tile : MonoBehaviour
{
    protected SpriteRenderer _spriteRenderer;
    protected SpriteAtlas _tileAtlas;
    protected TileManager _tileManager;
    protected TileType _tileType;
    protected int _x;
    protected int _y;

    public TileType TileType { get => _tileType; }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _tileAtlas = GenericSingleton<SpriteManager>.Instance.TileAtlas;
        _tileManager = GenericSingleton<TileManager>.Instance;
        SetSprite();
    }

    public virtual void Init(TileType tileType, int x, int y)
    {
        _tileType = tileType;
        _x = x;
        _y = y;
    }

    public virtual void TakeDamage() { }

    protected abstract void SetSprite();
    public abstract void DestroyTile();
}