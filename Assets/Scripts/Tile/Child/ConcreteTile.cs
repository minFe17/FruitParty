public class ConcreteTile : Tile
{
    public override void DestroyTile()
    {
        _tileManager.ConcreteTiles[_x, _y] = null;
        Destroy(this.gameObject);
    }
}
