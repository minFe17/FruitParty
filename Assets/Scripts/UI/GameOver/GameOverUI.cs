using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] GameObject _scorePanel;
    [SerializeField] Text _scoreText;

    ScoreManager _scoreManager;

    void Start()
    {
        _scoreManager = GenericSingleton<ScoreManager>.Instance;
        ShowScore();
        CheckNewHighScore();
    }

    void CheckNewHighScore()
    {

    }

    void ShowScore()
    {
        _scoreText.text = string.Format("{0:D2}", _scoreManager.Score);
    }

    public void Retry()
    {
        SceneManager.LoadScene("InGameScene");
    }

    public void ToLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
}
