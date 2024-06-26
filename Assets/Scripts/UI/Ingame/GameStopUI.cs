using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;
using Utils;

public class GameStopUI : MonoBehaviour
{
    [SerializeField]Animator _uiAnimator;

    [Header("Sprite")]
    [SerializeField] Image _gameStopPanel;
    [SerializeField] Image _playButton;
    [SerializeField] Image _playImage;
    [SerializeField] Image _lobbyButton;
    [SerializeField] Image _lobbyImage;

    GameManager _gameManager;
    SoundManager _soundManager;
    AudioClipManager _audioClipManager;
    CSVManager _csvManager;

    SpriteAtlas _uiAtlas;

    void Start()
    {
        _uiAtlas = GenericSingleton<SpriteManager>.Instance.UIAtlas;
        SetManager();
        SetSprite();
    }

    void SetManager()
    {
        _gameManager = GenericSingleton<GameManager>.Instance;
        _soundManager = GenericSingleton<SoundManager>.Instance;
        _audioClipManager = GenericSingleton<AudioClipManager>.Instance;
        _csvManager = GenericSingleton<CSVManager>.Instance;
    }

    void SetSprite()
    {
        _gameStopPanel.sprite = _uiAtlas.GetSprite("Option Panel");
        _playButton.sprite = _uiAtlas.GetSprite("Button");
        _playImage.sprite = _uiAtlas.GetSprite("Play");
        _lobbyButton.sprite = _uiAtlas.GetSprite("Button");
        _lobbyImage.sprite = _uiAtlas.GetSprite("ToLobby");
    }

    void MoveLobbyScene()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void Resume()
    {
        _soundManager.PlaySFX(_audioClipManager.ButtonSfX);
        _uiAnimator.SetBool("isStop", false);
        _gameManager.ChangeGameState(EGameStateType.Move);
        _csvManager.WriteSoundData();
    }

    public void ToLobby()
    {
        _soundManager.PlaySFX(_audioClipManager.ButtonSfX);
        _csvManager.WriteSoundData();
        Invoke("MoveLobbyScene", 0.1f);
    }
}