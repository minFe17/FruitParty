using UnityEngine;

public abstract class FactoryBase : MonoBehaviour
{
    protected FactoryManager _factoryManager;
    protected ObjectPoolManager _objectPoolManager;
    protected AddressableManager _addressableManager;
    protected GameObject _prefab;
}