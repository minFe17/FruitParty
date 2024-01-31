using System.Collections.Generic;
using UnityEngine;

public class AudioClipManager : MonoBehaviour
{
    // ╫л╠шео
    [Header("BGM")]
    AudioClip _inGameBGM;

    [Header("SFX")]
    List<AudioClip> _fruitMatchSFX = new List<AudioClip>();
    AudioClip _buttonSFX;
    AudioClip _gameOverSFX;

    public AudioClip InGameBGM { get => _inGameBGM; }
    public List<AudioClip> FruitMatchSFX { get => _fruitMatchSFX; }
    public AudioClip ButtonSfX { get => _buttonSFX; }
    public AudioClip GameOverSFX { get => _gameOverSFX; }

    public void Init()
    {
        _inGameBGM = Resources.Load("Prefabs/AudioClip/BGM") as AudioClip;
        AddMatchAudio();
        _buttonSFX = Resources.Load("Prefabs/AudioClip/Button") as AudioClip;
        _gameOverSFX = Resources.Load("Prefabs/AudioClip/GameOver") as AudioClip;
    }

    void AddMatchAudio()
    {
        _fruitMatchSFX.Add(Resources.Load("Prefabs/AudioClip/FruitMatch/FruitMatch1") as AudioClip);
        _fruitMatchSFX.Add(Resources.Load("Prefabs/AudioClip/FruitMatch/FruitMatch2") as AudioClip);
    }
}