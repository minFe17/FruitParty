public class LavaTile : Tile
{
    void Update()
    {
        if (_tileManager.LavaTiles[_x, _y] == null)
            DestroyTile();
    }

    protected override void SetSprite()
    {
        _spriteRenderer.sprite = _tileAtlas.GetSprite("LavaTile");
    }

    public override void DestroyTile()
    {
        _tileManager.LavaTiles[_x, _y] = null;
        _objectPoolManager.Pull(ETileKindType.Lava, gameObject);
    }
}