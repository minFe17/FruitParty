public class TileType
{
    ETileKindType _tileKindType;
    int _x;
    int _y;

    public ETileKindType TileKindType { get => _tileKindType; set => _tileKindType = value; }
    public int X { get => _x; set => _x = value; }
    public int Y { get => _y; set => _y = value;  }
}

public enum ETileKindType
{
    Normal,
    Blank,
    Ice,
}