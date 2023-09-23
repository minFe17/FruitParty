using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class LobbyUI : MonoBehaviour
{
    Animator _animator;
    SoundManager _soundManager;
    AudioClip _buttonAudio;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _soundManager = GenericSingleton<SoundManager>.Instance;
        _buttonAudio = Resources.Load("Prefabs/AudioClip/Button") as AudioClip;
        _soundManager.Init();
        _soundManager.CheckCsvFile();
        ReadHighScore();
    }

    void ReadHighScore()
    {
        // ScoreManager에 함수
        // 파일 읽기
        // 없으면 ---
    }

    void MoveGameScene()
    {
        SceneManager.LoadScene("InGameScene");
    }

    public void GameStart()
    {
        _soundManager.PlaySFX(_buttonAudio);
        Invoke("MoveGameScene", 0.1f);
    }

    public void OpenOption()
    {
        _soundManager.PlaySFX(_buttonAudio);
        _animator.SetBool("isOpenOption", true);
    }
}