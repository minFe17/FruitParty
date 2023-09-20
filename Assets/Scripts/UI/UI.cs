using UnityEngine;
using UnityEngine.UI;
using Utils;

public class UI : MonoBehaviour
{
    [Header("TimeUI")]
    [SerializeField] Image _timeBar;
    [SerializeField] Text _timeText;

    [Header("ScoreUI")]
    [SerializeField] Text _scoreText;

    [Header("EventUI")]
    [SerializeField] EventPanel _eventUI;

    [Header("GameOverUI")]
    [SerializeField] GameObject _gameOverUI;

    public void Init()
    {
        UIManager uiManager = GenericSingleton<UIManager>.Instance;
        uiManager.UI = this;
        uiManager.EventUI = _eventUI;
        uiManager.GameOverUI = _gameOverUI;
    }

    public void ShowScore()
    {
        int score = GenericSingleton<ScoreManager>.Instance.Score;
        _scoreText.text = score.ToString();
    }

    public void ShowRemainTime()
    {
        GameManager gameManager = GenericSingleton<GameManager>.Instance;
        float currentTime = gameManager.CurrentTime;
        float maxTime = gameManager.MaxTime;
        _timeBar.fillAmount = currentTime / maxTime;
        _timeText.text = string.Format("{0:0.#}", currentTime);
    }
}