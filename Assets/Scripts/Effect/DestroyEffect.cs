using UnityEngine;
using Utils;

public class DestroyEffect : MonoBehaviour
{
    ObjectPool<EEffectType> _effectObjectPool;

    void Awake()
    {
        _effectObjectPool = GenericSingleton<ObjectPool<EEffectType>>.Instance;
    }

    public void Init()
    {
        gameObject.SetActive(true);
    }

    void OnParticleSystemStopped()
    {
        _effectObjectPool.Pull(EEffectType.Destroy, gameObject);
    }
}