public class NormalTile : Tile
{
    protected override void SetSprite()
    {
        _spriteRenderer.sprite = _tileAtlas.GetSprite("Tile");
    }

    public override void DestroyTile()
    {
        _tileObjectPool.Pull(ETileKindType.Normal, gameObject);
    }
}