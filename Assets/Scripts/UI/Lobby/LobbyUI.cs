using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyUI : MonoBehaviour
{
    Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
        ReadHighScore();
    }

    void ReadHighScore()
    {

    }

    public void GameStart()
    {
        SceneManager.LoadScene("InGameScene");
    }

    public void OpenOption()
    {
        _animator.SetBool("isOpenOption", true);
    }

    public void CloseOption()
    {
        _animator.SetBool("isOpenOption", false);
    }
}