using UnityEngine;
using Utils;

public class ScoreManager : MonoBehaviour
{
    // ╫л╠шео
    CSVManager _csvManager;
    EventManager _eventManager;
    int _score;
    int _highScore;

    public int Score { get => _score; }
    public int HighScore { get => _highScore; set => _highScore = value; }

    void ShowScore()
    {
        UIManager uiManager = GenericSingleton<UIManager>.Instance;
        uiManager.GameUIPanel.ShowScore();
    }

    void CheckEvent()
    {
        if (_eventManager == null)
            _eventManager = GenericSingleton<EventManager>.Instance;
        _eventManager.OnEvent();
    }

    public void AddScore(int addScore)
    {
        _score += addScore;
        ShowScore();
        CheckEvent();
    }

    public bool CheckHighScore()
    {
        if (_score > _highScore)
        {
            _highScore = _score;
            _csvManager.WriteHighScoreData();
            return true;
        }
        return false;
    }

    public void ReadHighScoreData()
    {
        if (_csvManager == null)
            _csvManager = GenericSingleton<CSVManager>.Instance;
        _csvManager.ReadHighScoreData();
    }

    public void SetScore()
    {
        _score = 0;
    }
}