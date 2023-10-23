using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class GameStopUI : MonoBehaviour
{
    GameManager _gameManager;
    SoundManager _soundManager;
    AudioClipManager _audioClipManager;
    CSVManager _csvManager;

    Animator _uiAnimator;

    void Start()
    {
        _gameManager = GenericSingleton<GameManager>.Instance;
        _soundManager = GenericSingleton<SoundManager>.Instance;
        _audioClipManager = GenericSingleton<AudioClipManager>.Instance;
        _csvManager = GenericSingleton<CSVManager>.Instance;
        _uiAnimator = GenericSingleton<UIManager>.Instance.UI.UIAnimator;
    }

    void MoveLobbyScene()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void Resume()
    {
        _soundManager.PlaySFX(_audioClipManager.ButtonSfX);
        _uiAnimator.SetBool("isStop", false);
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