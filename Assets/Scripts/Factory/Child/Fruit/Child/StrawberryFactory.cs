using UnityEngine;

public class StrawberryFactory : FruitFactoryBase, IFactory<Fruit>
{
    protected override void Init()
    {
        _fruitType = EFruitType.Strawberry;
        _factoryManager.AddFactorys(_fruitType, this);
    }

    public Fruit MakeObject()
    {
        GameObject temp = _fruitObjectPool.Push(_fruitType, _prefab);
        return temp.GetComponent<Fruit>();
    }
}