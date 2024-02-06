using UnityEngine;

public class StarFriutFactory : FruitFactoryBase, IFactory<Fruit>
{
    protected override void Init()
    {
        _fruitType = EFruitType.StarFruit;
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