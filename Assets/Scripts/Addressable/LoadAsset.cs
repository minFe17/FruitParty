using System.Threading.Tasks;
using UnityEngine;
using Utils;

public class LoadAsset : MonoBehaviour
{
    // �̱���
    bool _isLoad;

    public async Task Init()
    {
        if (!_isLoad)
        {
            await LoadLobbyAsset();
            LoadIngameAsset();
            _isLoad = true;
        }
    }

    async Task LoadLobbyAsset()
    {
        await GenericSingleton<SpriteManager>.Instance.Init();
        await GenericSingleton<UIManager>.Instance.LoadAsset();
        await GenericSingleton<SoundManager>.Instance.Init();
        await GenericSingleton<AudioClipManager>.Instance.Init();
    }

    void LoadIngameAsset()
    {
        GenericSingleton<EventManager>.Instance.LoadAsset();
        GenericSingleton<FruitManager>.Instance.LoadAsset();
        GenericSingleton<BombManager>.Instance.LoadAsset();
        GenericSingleton<EffectManager>.Instance.LoadAsset();
        GenericSingleton<TileManager>.Instance.LoadAsset();
    }
}