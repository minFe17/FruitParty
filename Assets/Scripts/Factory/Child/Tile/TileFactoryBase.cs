using UnityEngine;
using Utils;

public abstract class TileFactoryBase : FactoryBase
{
    protected FactoryManager<ETileKindType, Tile> _factoryManager;
    protected ObjectPool<ETileKindType> _tileObjectPool;
    protected ETileKindType _tileType;

    protected abstract void Init();

    async void Awake()
    {
        _factoryManager = GenericSingleton<FactoryManager<ETileKindType, Tile>>.Instance;
        _tileObjectPool = GenericSingleton<ObjectPool<ETileKindType>>.Instance;
        _addressableManager = GenericSingleton<AddressableManager>.Instance;
        Init();
        _prefab = await _addressableManager.GetAddressableAsset<GameObject>($"{_tileType}Tile");
    }
}
