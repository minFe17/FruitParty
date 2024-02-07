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
        _objectPoolManager.Pull(EEffectType.Destroy, gameObject);
    }
}