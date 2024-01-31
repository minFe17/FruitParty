using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] Text _highScore;

    Animator _animator;
    SoundManager _soundManager;
    AudioClipManager _audioClipManager;
    ScoreManager _scoreManager;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _soundManager = GenericSingleton<SoundManager>.Instance;
        _audioClipManager = GenericSingleton<AudioClipManager>.Instance;
        _scoreManager = GenericSingleton<ScoreManager>.Instance;
        _soundManager.Init();
        _audioClipManager.Init();
        _soundManager.StartBGM(_audioClipManager.LobbyBGM);
        ReadHighScore();
    }

    void ReadHighScore()
    {
        _scoreManager.ReadHighScoreData();

        if (_scoreManager.HighScore != 0)
            _highScore.text = _scoreManager.HighScore.ToString();
        else
            _highScore.text = "---";
    }

    void MoveGameScene()
    {
        SceneManager.LoadScene("InGameScene");
    }

    public void GameStart()
    {
        _soundManager.PlaySFX(_audioClipManager.ButtonSfX);
        Invoke("MoveGameScene", 0.1f);
    }

    public void OpenOption()
    {
        _soundManager.PlaySFX(_audioClipManager.ButtonSfX);
        _animator.SetBool("isOpenOption", true);
    }
}