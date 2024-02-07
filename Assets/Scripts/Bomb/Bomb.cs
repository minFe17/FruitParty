public abstract class Bomb : Fruit
{
    protected bool _isUse;

    public override void Init()
    {
        base.Init();
        _isBomb = true;
        _isUse = false;
    }

    protected override void Update()
    {
        base.Update();
        if (_isMatch)
            OnEffect();
    }

    public void ChangeColor()
    {
        _colorType = _factoryManager.ColorType;
    }

    public abstract void OnEffect();
}

public enum EBombType
{
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