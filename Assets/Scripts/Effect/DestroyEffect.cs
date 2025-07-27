using UnityEngine;
using Utils;

public class DestroyEffect : MonoBehaviour
{
    ObjectPoolManager _objectPoolManager;

    void Awake()
    {
        _objectPoolManager = GenericSingleton<ObjectPoolManager>.Instance;
    }

    public void Init()
    {
        gameObject.SetActive(true);
    }

    void OnParticleSystemStopped()
    {
        _objectPoolManager.Push(EEffectType.Destroy, gameObject);
    }
}