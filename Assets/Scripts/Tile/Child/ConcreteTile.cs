public class ConcreteTile : Tile
{
    void Update()
    {
        if (_tileManager.ConcreteTiles[_x, _y] == null)
            DestroyTile();
    }

    protected override void SetSprite()
    {
        _spriteRenderer.sprite = _tileAtlas.GetSprite("ConcreteTile");
    }

    public override void DestroyTile()
    {
        _tileManager.ConcreteTiles[_x, _y] = null;
        Destroy(this.gameObject);
    }
}