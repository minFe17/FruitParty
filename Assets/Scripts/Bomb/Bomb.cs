public class Bomb : Fruit
{
    protected EBombType _bombType;
    public EBombType BombType { get => _bombType; }

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
    LineBomb,
    Max,
}

public enum ELineBombDirectionType
{
    None,
    Column,
    Row,
    Max,
}