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

    [Header("EventImage")]
    [SerializeField] Image _shuffleImage;

    public void ShowScore()
    {
        int score = GenericSingleton<ScoreManager>.Instance.Score;
        _scoreText.text = score.ToString();
    }
}