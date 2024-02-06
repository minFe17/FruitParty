using UnityEngine;
using Utils;

public abstract class BombFactoryBase : FactoryBase
{
    protected FactoryManager<EBombType, Bomb> _factoryManager;
    protected ObjectPool<EBombType> _bombObjectPool;
    protected EBombType _bombType;

    protected abstract void Init();

    async void Awake()
    {
        _factoryManager = GenericSingleton<FactoryManager<EBombType, Bomb>>.Instance;
        _bombObjectPool = GenericSingleton<ObjectPool<EBombType>>.Instance;
        _addressableManager = GenericSingleton<AddressableManager>.Instance;
        Init();
        _prefab = await _addressableManager.GetAddressableAsset<GameObject>(_bombType.ToString());
    }
}