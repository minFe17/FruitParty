public class LockTile : Tile
{
    public override void DestroyTile()
    {
        _tileManager.LockTiles[_x, _y] = null;
        Destroy(this.gameObject);
    }
}
