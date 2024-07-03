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
    List<AudioClip> _sfxAudios = new List<AudioClip>();

    AddressableManager _addressableManager;
    SoundManager _soundManager;

    public async Task Init()
    {
        _addressableManager = GenericSingleton<AddressableManager>.Instance;
        _soundManager = GenericSingleton<SoundManager>.Instance;
        await LoadAsset();
    }

    async Task LoadAsset()
    {
        _inGameBGM = await _addressableManager.GetAddressableAsset<AudioClip>("BGM");
        await SetSFX();
    }

    async Task SetSFX()
    {
        for (int i = 0; i < (int)EFruitMatchSFXType.Max; i++)
        {
            string matchSoundName = ((EFruitMatchSFXType)i).ToString();
            AudioClip audioSound = await _addressableManager.GetAddressableAsset<AudioClip>(matchSoundName);
            _sfxAudios.Add(audioSound);
        }

        for (int i = 0; i < (int)ESFXSoundType.Max; i++)
        {
            string audioSoundName = ((ESFXSoundType)i).ToString();
            AudioClip audioSound = await _addressableManager.GetAddressableAsset<AudioClip>(audioSoundName);
            _sfxAudios.Add(audioSound);
        }
    }

    public void PlayBGM()
    {
        _soundManager.StartBGM(_inGameBGM);
    }

    public void StopBGM()
    {
        _soundManager.StopBGM();
    }

    public void PlayMatchSFX()
    {
        int randomSound = Random.Range(0, (int)EFruitMatchSFXType.Max);
        _soundManager.PlaySFX(_sfxAudios[randomSound]);
    }

    public void PlaySFX(ESFXSoundType soundType)
    {
        _soundManager.PlaySFX(_sfxAudios[(int)soundType]);
    }
}