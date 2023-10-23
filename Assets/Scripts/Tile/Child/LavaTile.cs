public class LavaTile : Tile
{
    public override void DestroyTile()
    {
        _tileManager.LavaTiles[_x, _y] = null;
        Destroy(this.gameObject);
    }
}
