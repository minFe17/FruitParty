public class Bomb : Fruit
{
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

    public virtual void OnEffect()
    {

    }
}

public enum EBombType
{
    None,
    LineBomb,
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