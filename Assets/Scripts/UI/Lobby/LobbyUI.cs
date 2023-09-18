using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] GameObject _otherUI;

    void Start()
    {
        _otherUI.SetActive(false);
    }

    void ReadHighScore()
    {

    }

    public void ShowScoreAndPlayButton()
    {
        ReadHighScore();
        _otherUI.SetActive(true);
    }

    public void GameStart()
    {
        SceneManager.LoadScene("InGameScene");
    }
}