using UnityEngine;

public class DestroyEffectFactory : EffectFactoryBase, IFactory<GameObject>
{
    protected override void Init()
    {
        _effectType = EEffectType.Destroy;
        _factoryManager.AddFactorys(_effectType, this);
    }

    public GameObject MakeObject(Vector2Int position)
    {
        GameObject temp = _effectObjectPool.Push(_effectType, _prefab);
        temp.transform.position = (Vector2)position;
        temp.GetComponent<DestroyEffect>().Init();
        return temp;
    }
}