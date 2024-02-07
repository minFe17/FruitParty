using UnityEngine;
using Utils;

public abstract class EffectFactoryBase : FactoryBase
{
    protected EEffectType _effectType;

    protected abstract void Init();

    async void Start()
    {
        _factoryManager = GenericSingleton<FactoryManager>.Instance;
        _objectPoolManager = GenericSingleton<ObjectPoolManager>.Instance;
        _addressableManager = GenericSingleton<AddressableManager>.Instance;
        Init();
        _prefab = await _addressableManager.GetAddressableAsset<GameObject>($"{_effectType}Effect");
    }
}