using UnityEngine;
using Utils;

public abstract class FruitFactoryBase : FactoryBase
{
    protected FactoryManager<EFruitType, Fruit> _factoryManager;
    protected ObjectPool<EFruitType> _fruitObjectPool;
    protected EFruitType _fruitType;

    protected abstract void Init();

    async void Awake()
    {
        _factoryManager = GenericSingleton<FactoryManager<EFruitType, Fruit>>.Instance;
        _fruitObjectPool = GenericSingleton<ObjectPool<EFruitType>>.Instance;
        _addressableManager = GenericSingleton<AddressableManager>.Instance;
        Init();
        _prefab = await _addressableManager.GetAddressableAsset<GameObject>(_fruitType.ToString());
    }

    protected virtual void Make()
    {

    }
}
