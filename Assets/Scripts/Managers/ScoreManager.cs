using UnityEngine;
using Utils;

public class ScoreManager : MonoBehaviour
{
    // �̱���
    int _score;

    public int Score { get => _score; }

    public void AddScore(int addScore)
    {
        _score += addScore;
        GenericSingleton<UIManager>.Instance.UI.ShowScoreText();
    }
}