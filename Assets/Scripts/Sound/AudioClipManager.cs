using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

public class AudioClipManager : MonoBehaviour
{
    // ╫л╠шео
    [Header("BGM")]
    AudioClip _inGameBGM;

    [Header("SFX")]
    List<AudioClip> _fruitMatchSFX = new List<AudioClip>();
    AudioClip _buttonSFX;
    AudioClip _gameOverSFX;

    AddressableManager _addressableManager;

    public AudioClip InGameBGM { get => _inGameBGM; }
    public List<AudioClip> FruitMatchSFX { get => _fruitMatchSFX; }
    public AudioClip ButtonSfX { get => _buttonSFX; }
    public AudioClip GameOverSFX { get => _gameOverSFX; }

    public async Task Init()
    {
        _addressableManager = GenericSingleton<AddressableManager>.Instance;
        await LoadAsset();
        await AddMatchAudio();
    }

    async Task LoadAsset()
    {
        _inGameBGM = await _addressableManager.GetAddressableAsset<AudioClip>("BGM");
        _buttonSFX = await _addressableManager.GetAddressableAsset<AudioClip>("Button");
        _gameOverSFX = await _addressableManager.GetAddressableAsset<AudioClip>("GameOver");
    }

    async Task AddMatchAudio()
    {
        _fruitMatchSFX.Add(await _addressableManager.GetAddressableAsset<AudioClip>("FruitMatch1"));
        _fruitMatchSFX.Add(await _addressableManager.GetAddressableAsset<AudioClip>("FruitMatch2"));
    }
}