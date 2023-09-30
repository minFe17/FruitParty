public class LockTile : Tile
{
    public void DestroyTile()
    {
        _tileManager.LockTiles[_x, _y] = null;
        Destroy(this.gameObject);
    }
}
