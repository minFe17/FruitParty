using UnityEngine;

public class CarrotFactory : FruitFactoryBase, IFactory<Fruit>
{
    protected override void Init()
    {
        _fruitType = EFruitType.Carrot;
        _factoryManager.AddFactorys(_fruitType, this);
    }

    public Fruit MakeObject(Vector2Int position)
    {
        GameObject temp = _fruitObjectPool.Push(_fruitType, _prefab);
        Fruit fruit = temp.GetComponent<Fruit>();
        fruit.Column = position.x;
        fruit.Row = position.y;
        return fruit;
    }
}