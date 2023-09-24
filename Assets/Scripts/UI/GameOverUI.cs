using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

public class GameOverUI : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] GameObject _scorePanel;
    [SerializeField] Text _scoreText;

    [Header("NewHighScore")]
    [SerializeField] GameObject _newHighScore;

    ScoreManager _scoreManager;
    SoundManager _soundManager;
    AudioClipManager _audioClipManager;

    string _sceneName;

    void Start()
    {
        _scoreManager = GenericSingleton<ScoreManager>.Instance;
        _soundManager = GenericSingleton<SoundManager>.Instance;
        _audioClipManager = GenericSingleton<AudioClipManager>.Instance;
        ShowScore();
        CheckNewHighScore();
        _soundManager.StopBGM();
        _soundManager.PlaySFX(_audioClipManager.GameOverSFX);
    }

    void CheckNewHighScore()
    {
        if (_scoreManager.CheckHighScore())
            _newHighScore.SetActive(true);
    }

    void ShowScore()
    {
        _scoreText.text = string.Format("{0:D2}", _scoreManager.Score);
    }

    void ChangeScene()
    {
        SceneManager.LoadScene(_sceneName);
    }

    public void Retry()
    {
        _soundManager.PlaySFX(_audioClipManager.ButtonSfX);
        _sceneName = "InGameScene";
        Invoke("ChangeScene", 0.1f);
    }

    public void ToLobby()
    {
        _soundManager.PlaySFX(_audioClipManager.ButtonSfX);
        _sceneName = "Lobby";
        Invoke("ChangeScene", 0.1f);
    }
}