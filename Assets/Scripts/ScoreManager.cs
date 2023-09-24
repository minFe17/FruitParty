using UnityEngine;
using Utils;

public class ScoreManager : MonoBehaviour
{
    // ╫л╠шео
    CSVManager _csvManager;
    int _score;
    int _highScore;

    public int Score { get => _score; }
    public int HighScore { get => _highScore; set => _highScore = value; }

    public void AddScore(int addScore)
    {
        UIManager uiManager = GenericSingleton<UIManager>.Instance;
        _score += addScore;
        uiManager.UI.ShowScore();
    }

    public bool CheckHighScore()
    {
        if (_score > _highScore)
        {
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
}