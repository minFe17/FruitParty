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
    AudioClip _buttonAudio;

    string _sceneName;

    void Start()
    {
        _scoreManager = GenericSingleton<ScoreManager>.Instance;
        _soundManager = GenericSingleton<SoundManager>.Instance;
        _buttonAudio = Resources.Load("Prefabs/AudioClip/Button") as AudioClip;
        ShowScore();
        CheckNewHighScore();
    }

    void CheckNewHighScore()
    {
        // ScoreManager 속성
        // 최고점수보다 높으면 신기록!! UI보여준 후 파일 쓰기
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
        _soundManager.PlaySFX(_buttonAudio);
        _sceneName = "InGameScene";
        Invoke("ChangeScene", 0.1f);
    }

    public void ToLobby()
    {
        _soundManager.PlaySFX(_buttonAudio);
        _sceneName = "Lobby";
        Invoke("ChangeScene", 0.1f);
    }
}