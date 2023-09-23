using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class GameStopUI : MonoBehaviour
{
    [SerializeField] Animator _animator;

    GameManager _gameManager;
    SoundManager _soundManager;
    AudioClip _buttonAudio;

    void Start()
    {
        _gameManager = GenericSingleton<GameManager>.Instance;
        _soundManager = GenericSingleton<SoundManager>.Instance;
        _buttonAudio = Resources.Load("Prefabs/AudioClip/Button") as AudioClip;
    }

    void MoveLobbyScene()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void Resume()
    {
        _soundManager.PlaySFX(_buttonAudio);
        _animator.SetBool("isStop", false);
        _gameManager.GameState = EGameStateType.Move;
        //파일 쓰기
    }

    public void ToLobby()
    {
        _soundManager.PlaySFX(_buttonAudio);
        // 파일 쓰기
        Invoke("MoveLobbyScene", 0.1f);
    }
}