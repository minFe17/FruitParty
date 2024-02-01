using UnityEngine;
using Utils;

public class EffectManager : MonoBehaviour
{
    // ╫л╠шео

    GameObject _destroyEffect;
    GameObject _hintEffect;

    AddressableManager _addressableManager;

    public GameObject DestroyEffect { get => _destroyEffect; }
    public GameObject HintEffect { get => _hintEffect; }

    public async void LoadAsset()
    {
        if (_addressableManager == null)
            _addressableManager = GenericSingleton<AddressableManager>.Instance;
        _destroyEffect = await _addressableManager.GetAddressableAsset<GameObject>("DestroyEffect");
        _hintEffect = await _addressableManager.GetAddressableAsset<GameObject>("HintEffect");
    }
}
