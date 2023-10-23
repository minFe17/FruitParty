public class LavaTile : Tile
{
    void Update()
    {
        if (_tileManager.LavaTiles[_x, _y] == null)
            Destroy(this.gameObject);
    }

    public override void DestroyTile()
    {
        _tileManager.LavaTiles[_x, _y] = null;
        Destroy(this.gameObject);
    }
}