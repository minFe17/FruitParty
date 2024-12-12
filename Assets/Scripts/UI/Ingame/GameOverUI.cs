using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

public class GameOverUI : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] Text _scoreText;

    [Header("NewHighScore")]
    [SerializeField] GameObject _newHighScore;

    [Header("Sprite")]
    [SerializeField] Image _gameoverPanel;
    [SerializeField] Image _scorePanel;
    [SerializeField] Image _retryButton;
    [SerializeField] Image _retryImage;
    [SerializeField] Image _lobbyButton;
    [SerializeField] Image _lobbyImage;

    FruitManager _fruitManager;
    TileManager _tileManager;
    SpriteManager _spriteManager;
    ScoreManager _scoreManager;
    AudioClipManager _audioClipManager;

    string _sceneName;

    void Start()
    {
        SetManager();
        SetSprite();
        ShowScore();
        CheckNewHighScore();
        _audioClipManager.StopBGM();
        _audioClipManager.PlaySFX(ESFXSoundType.GameOver);
    }

    void SetManager()
    {
        _fruitManager = GenericSingleton<FruitManager>.Instance;
        _tileManager = GenericSingleton<TileManager>.Instance;
        _spriteManager = GenericSingleton<SpriteManager>.Instance;
        _scoreManager = GenericSingleton<ScoreManager>.Instance;
        _audioClipManager = GenericSingleton<AudioClipManager>.Instance;
    }

    void SetSprite()
    {
        _gameoverPanel.sprite = _spriteManager.BackgroundSprite;
        _scorePanel.sprite = _spriteManager.UIAtlas.GetSprite("Option Panel");
        _retryButton.sprite = _spriteManager.UIAtlas.GetSprite("Button");
        _retryImage.sprite = _spriteManager.UIAtlas.GetSprite("Retry");
        _lobbyButton.sprite = _spriteManager.UIAtlas.GetSprite("Button");
        _lobbyImage.sprite = _spriteManager.UIAtlas.GetSprite("ToLobby");
    }

    void CheckNewHighScore()
    {
        if (_scoreManager.CheckHighScore())
            _newHighScore.SetActive(true);
    }

    void ShowScore()
    {
        _scoreText.text = string.Format("{0:D2}", _scoreManager.Score);
    }

    void ChangeScene()
    {
        SceneManager.LoadScene(_sceneName);
    }

    public void Retry()
    {
        _fruitManager.DestroyAllFruit();
        _tileManager.DestroyAllTile();
        _audioClipManager.PlaySFX(ESFXSoundType.Button);
        _sceneName = "InGameScene";
        Invoke("ChangeScene", 0.1f);
    }

    public void ToLobby()
    {
        _fruitManager.DestroyAllFruit();
        _tileManager.DestroyAllTile();
        _audioClipManager.PlaySFX(ESFXSoundType.Button);
        _sceneName = "Lobby";
        Invoke("ChangeScene", 0.1f);
    }
}