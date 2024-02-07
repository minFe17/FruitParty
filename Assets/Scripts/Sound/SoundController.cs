using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] AudioSource _bgm;
    [SerializeField] List<AudioSource> _sfx;

    public AudioSource BGM { get => _bgm; }
    public List<AudioSource> SFX { get => _sfx; }
}