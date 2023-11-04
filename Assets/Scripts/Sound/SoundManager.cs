using UnityEngine;
using Utils;

public class SoundManager : MonoBehaviour
{
    // ╫л╠шео
    CSVManager _csvManager;
    SoundController _soundController;

    int _index;
    float _bgmSound;
    float _sfxSound;

    public float BgmSound { get { return _bgmSound; } set { _bgmSound = value; } }
    public float SFXSound { get { return _sfxSound; } set { _sfxSound = value; } }

    public void Init()
    {
        _csvManager = GenericSingleton<CSVManager>.Instance;
        _csvManager.ReadSoundData();

        _index = 0;
        if (_soundController == null)
            CreateSoundController();

        ChangeBGMVolumn();
        ChangeSFXVolumn();
    }

    void CreateSoundController()
    {
        GameObject temp = Resources.Load("Prefabs/SoundController") as GameObject;
        GameObject soundController = Instantiate(temp);
        _soundController = soundController.GetComponent<SoundController>();
    }

    public void StartBGM(AudioClip audio)
    {
        _soundController.BGM.clip = audio;
        _soundController.BGM.Play();
    }

    public void StopBGM()
    {
        _soundController.BGM.Stop();
    }

    public void PlaySFX(AudioClip audio)
    {
        _soundController.SFX[_index].clip = audio;
        _soundController.SFX[_index].Play();
        _index++;
        if (_index == _soundController.SFX.Count)
            _index = 0;
    }

    public void ChangeBGMVolumn()
    {
        _soundController.BGM.volume = _bgmSound;
    }

    public void ChangeSFXVolumn()
    {
        for (int i = 0; i < _soundController.SFX.Count; i++)
            _soundController.SFX[i].volume = _sfxSound;
    }
}