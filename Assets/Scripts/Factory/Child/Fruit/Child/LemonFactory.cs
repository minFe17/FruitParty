using UnityEngine;

public class LemonFactory : FruitFactoryBase, IFactory<Fruit>
{
    protected override void Init()
    {
        _fruitType = EFruitType.Lemon;
        _factoryManager.AddFactorys(_fruitType, this);
    }

    public Fruit MakeObject(Vector2Int position)
    {
        GameObject temp = _objectPoolManager.Pull(_fruitType, _prefab);
        Fruit fruit = temp.GetComponent<Fruit>();
        fruit.Column = position.x;
        fruit.Row = position.y;
        return fruit;
    }
}