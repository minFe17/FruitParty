using UnityEngine;

public class HintEffectFactory : EffectFactoryBase, IFactory<GameObject>
{
    protected override void Init()
    {
        _effectType = EEffectType.Hint;
        _factoryManager.AddFactorys(_effectType, this);
    }

    public GameObject MakeObject(Vector2Int position)
    {
        GameObject temp = _objectPoolManager.Push(_effectType, _prefab);
        temp.transform.position = (Vector2)position;
        return temp;
    }
}