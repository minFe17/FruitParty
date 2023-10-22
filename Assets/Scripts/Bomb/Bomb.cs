public abstract class Bomb : Fruit
{
    protected bool _isUse;

    void Start()
    {
        _isBomb = true;
    }

    protected override void Update()
    {
        base.Update();
        if (_isMatch)
            OnEffect();
    }

    public abstract void OnEffect();
}

public enum EBombType
{
    None,
    LineBomb,
    SquareBomb,
    FruitBomb,
    Max,
}

public enum ELineBombDirectionType
{
    None,
    Column,
    Row,
    Max,
}