using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class GameStopUI : MonoBehaviour
{
    [SerializeField] Animator _animator;

    GameManager _gameManager;
    SoundManager _soundManager;
    AudioClipManager _audioClipManager;
    CSVManager _csvManager;

    void Start()
    {
        _gameManager = GenericSingleton<GameManager>.Instance;
        _soundManager = GenericSingleton<SoundManager>.Instance;
        _csvManager = GenericSingleton<CSVManager>.Instance;
        _audioClipManager = GenericSingleton<AudioClipManager>.Instance;
    }

    void MoveLobbyScene()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void Resume()
    {
        _soundManager.PlaySFX(_audioClipManager.ButtonSfX);
        _animator.SetBool("isStop", false);
        _gameManager.GameState = EGameStateType.Move;
        _csvManager.WriteSoundData();
    }

    public void ToLobby()
    {
        _soundManager.PlaySFX(_audioClipManager.ButtonSfX);
        _csvManager.WriteSoundData();
        Invoke("MoveLobbyScene", 0.1f);
    }
}