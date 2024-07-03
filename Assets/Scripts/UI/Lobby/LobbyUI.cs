using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] Text _highScore;

    Animator _animator;
    AudioClipManager _audioClipManager;
    ScoreManager _scoreManager;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _audioClipManager = GenericSingleton<AudioClipManager>.Instance;
        _scoreManager = GenericSingleton<ScoreManager>.Instance;
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
        _audioClipManager.PlaySFX(ESFXSoundType.Button);
        Invoke("MoveGameScene", 0.1f);
    }

    public void OpenOption()
    {
        _audioClipManager.PlaySFX(ESFXSoundType.Button);
        _animator.SetBool("isOpenOption", true);
    }
}