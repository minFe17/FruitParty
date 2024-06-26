using System.Collections.Generic;

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

    protected virtual void CheckHitTile() { }

    protected virtual void GetFruits(out List<Fruit> fruits) 
    {  
        fruits = new List<Fruit>(); 
    }

    public abstract void OnEffect();
}