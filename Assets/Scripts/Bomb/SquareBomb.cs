using System.Linq;
using System.Text;

public class SquareBomb : Bomb
{
    StringBuilder _stringBuilder = new StringBuilder();

    protected override void Start()
    {
        base.Start();
        _bombType = EBombType.SquareBomb;
    }

    protected override void SetSprite()
    {
        string color = _colorType.ToString();
        _stringBuilder.Append(color);
        _stringBuilder.Append("SquareBomb");
        _spriteRenderer.sprite = _spriteManager.BombAtlas.GetSprite(_stringBuilder.ToString());
    }

    public override void OnEffect()
    {
        if (!_isUse)
        {
            _matchFinder.MatchFruits.Union(_bombManager.GetSquareFruits(_column, _row));
            _bombManager.HitTileSquareBomb(_column, _row);
            _isUse = true;
        }
    }
}