using UnityEngine;

public class LemonFactory : FruitFactoryBase, IFactory<Fruit>
{
    protected override void Init()
    {
        _fruitType = EFruitType.Lemon;
        _factoryManager.AddFactorys(_fruitType, this);
    }

    public Fruit MakeObject()
    {
        GameObject temp = _fruitObjectPool.Push(_fruitType, _prefab);
        return temp.GetComponent<Fruit>();
    }
}