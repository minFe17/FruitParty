using UnityEngine;
using Utils;

public abstract class EffectFactoryBase : FactoryBase
{
    protected FactoryManager<EEffectType, GameObject> _factoryManager;
    protected ObjectPool<EEffectType> _effectObjectPool;
    protected EEffectType _effectType;

    protected abstract void Init();

    async void Awake()
    {
        _factoryManager = GenericSingleton<FactoryManager<EEffectType, GameObject>>.Instance;
        _effectObjectPool = GenericSingleton<ObjectPool<EEffectType>>.Instance;
        _addressableManager = GenericSingleton<AddressableManager>.Instance;
        Init();
        _prefab = await _addressableManager.GetAddressableAsset<GameObject>($"{_effectType}Effect");
    }
}
