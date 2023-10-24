public class ConcreteTile : Tile
{
    void Update()
    {
        if (_tileManager.ConcreteTiles[_x, _y] == null)
            Destroy(this.gameObject);
    }

    public override void DestroyTile()
    {
        _tileManager.ConcreteTiles[_x, _y] = null;
        Destroy(this.gameObject);
    }
}