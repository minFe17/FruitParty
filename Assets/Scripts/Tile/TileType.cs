public struct TileType
{
    ETileKindType _tileKindType;
    Tile _tile;
    int _x;
    int _y;

    public ETileKindType TileKindType { get => _tileKindType; }
    public Tile Tile { get => _tile; }
    public int X { get => _x; }
    public int Y { get => _y; }

    public TileType(ETileKindType tileKindType, Tile tile, int x, int y)
    {
        _tileKindType = tileKindType;
        _tile = tile;
        _x = x;
        _y = y;
    }
}