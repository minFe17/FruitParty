using UnityEngine;

public class CarrotFactory : FruitFactoryBase, IFactory<Fruit>
{
    protected override void Init()
    {
        _fruitType = EFruitType.Carrot;
        _factoryManager.AddFactorys(_fruitType, this);
    }

    public Fruit MakeObject()
    {
        GameObject temp = _fruitObjectPool.Push(_fruitType, _prefab);
        return temp.GetComponent<Fruit>();
    }
}