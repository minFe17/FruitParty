public class NormalTile : Tile
{
    protected override void SetSprite()
    {
        _spriteRenderer.sprite = _tileAtlas.GetSprite("Tile");
    }

    public override void DestroyTile()
    {
        _objectPoolManager.Push(ETileKindType.Normal, gameObject);
    }
}