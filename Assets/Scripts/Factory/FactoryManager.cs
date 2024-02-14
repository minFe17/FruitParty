using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

public class FactoryManager : MonoBehaviour
{
    //╫л╠шео
    Dictionary<Type, IFactorys> _factorys = new Dictionary<Type, IFactorys>();
    Factory<Fruit> _fruitFactory = new Factory<Fruit>();
    GameObject _factoryPrefab;
    AddressableManager _addressableManager;

    public EColorType ColorType { get; set; }
    public int FruitCount { get => _fruitFactory.Factorys.Count; }
    
    void Awake()
    {
        _factorys.Add(typeof(EFruitType), _fruitFactory);
        _factorys.Add(typeof(EBombType), new Factory<Bomb>());
        _factorys.Add(typeof(ETileKindType), new Factory<Tile>());
        _factorys.Add(typeof(EEffectType), new Factory<GameObject>());
    }

    public async void Init()
    {
        await LoadAsset();
        CreateFactory();
    }

    async Task LoadAsset()
    {
        if (_factoryPrefab == null)
        {
            if (_addressableManager == null)
                _addressableManager = GenericSingleton<AddressableManager>.Instance;
            _factoryPrefab = await _addressableManager.GetAddressableAsset<GameObject>("Factory");
        }
    }

    void CreateFactory()
    {
        Instantiate(_factoryPrefab, transform);
    }

    public void AddFactorys<TEnum, T>(TEnum key, IFactory<T> value) where TEnum : Enum
    {
        IFactorys factory;
        _factorys.TryGetValue(typeof(TEnum), out factory);
        factory.AddFactorys(key, value);
    }

    public T MakeObject<TEnum, T>(TEnum key, Vector2Int position) where TEnum : Enum
    {
        IFactorys factory;
        _factorys.TryGetValue(typeof(TEnum), out factory);
        return (T)factory.MakeObject(key, position);
    }
}