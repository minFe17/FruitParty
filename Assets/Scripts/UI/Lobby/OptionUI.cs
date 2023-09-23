using UnityEngine;
using Utils;

public class OptionUI : MonoBehaviour
{
    [SerializeField] Animator _animator;

    SoundManager _soundManager;
    AudioClip _buttonAudio;

    void Start()
    {
        _soundManager = GenericSingleton<SoundManager>.Instance;
        _buttonAudio = Resources.Load("Prefabs/AudioClip/Button") as AudioClip;
    }

    public void CloseOption()
    {
        _soundManager.PlaySFX(_buttonAudio);
        _animator.SetBool("isOpenOption", false);
        // 파일쓰기
    }
}