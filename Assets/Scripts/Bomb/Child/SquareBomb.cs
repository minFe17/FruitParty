using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SquareBomb : Bomb
{
    StringBuilder _stringBuilder = new StringBuilder();

    protected override void Awake()
    {
        base.Awake();
        _bombType = EBombType.SquareBomb;
    }

    protected override void SetSprite()
    {
        _stringBuilder.Clear();
        string color = _colorType.ToString();
        _stringBuilder.Append(color);
        _stringBuilder.Append("SquareBomb");
        _spriteRenderer.sprite = _spriteManager.BombAtlas.GetSprite(_stringBuilder.ToString());
    }

    public override void OnEffect()
    {
        if (!_isUse)
        {
            List<Fruit> fruits;
            _bombManager.GetSquareFruits(_column, _row, out fruits);
            _matchFinder.MatchFruits.Union(fruits);
            _bombManager.HitTileSquareBomb(_column, _row);
            _isUse = true;
        }
    }
}