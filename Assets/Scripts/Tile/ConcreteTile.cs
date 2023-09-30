public class ConcreteTile : Tile
{
    public void DestroyTile()
    {
        _tileManager.ConcreteTiles[_x, _y] = null;
        Destroy(this.gameObject);
    }
}
