using UnityEngine;
using Utils;

public class OptionUI : MonoBehaviour
{
    [SerializeField] Animator _animator;

    SoundManager _soundManager;
    AudioClipManager _audioClipManager;
    CSVManager _csvManager;

    void Start()
    {
        _soundManager = GenericSingleton<SoundManager>.Instance;
        _csvManager = GenericSingleton<CSVManager>.Instance;
        _audioClipManager = GenericSingleton<AudioClipManager>.Instance;
    }

    public void CloseOption()
    {
        _soundManager.PlaySFX(_audioClipManager.ButtonSfX);
        _animator.SetBool("isOpenOption", false);
        _csvManager.WriteSoundData();
    }
}