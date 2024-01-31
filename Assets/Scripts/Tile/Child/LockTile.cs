public class LockTile : Tile
{
    void Update()
    {
        if (_tileManager.LockTiles[_x, _y] == null)
            Destroy(this.gameObject);
    }

    protected override void SetSprite()
    {
        _spriteRenderer.sprite = _tileAtlas.GetSprite("LockTile");
    }

    public override void DestroyTile()
    {
        _tileManager.LockTiles[_x, _y] = null;
        Destroy(this.gameObject);
    }
}