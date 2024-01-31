using System.Threading.Tasks;
using UnityEngine;
using Utils;

public class LoadAsset : MonoBehaviour
{
    // ╫л╠шео
    public async Task Init()
    {
        await LoadLobbyAsset();
    }

    async Task LoadLobbyAsset()
    {
        await GenericSingleton<SpriteManager>.Instance.Init();
        await GenericSingleton<UIManager>.Instance.LoadAsset();
        await GenericSingleton<SoundManager>.Instance.Init();
        await GenericSingleton<AudioClipManager>.Instance.Init();
    }
}