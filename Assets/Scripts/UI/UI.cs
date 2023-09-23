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

    [Header("EventUI")]
    [SerializeField] EventPanel _eventUI;

    [Header("GameStopUI")]
    [SerializeField] GameObject _stopUI;

    [Header("GameOverUI")]
    [SerializeField] GameObject _gameOverUI;

    GameManager _gameManager;
    SoundManager _soundManager;
    Animator _animator;
    AudioClip _buttonAudio;

    void Start()
    {
        _gameManager = GenericSingleton<GameManager>.Instance;
        _soundManager = GenericSingleton<SoundManager>.Instance;
        _animator = GetComponent<Animator>();
        _buttonAudio = Resources.Load("Prefabs/AudioClip/Button") as AudioClip;
    }

    public void Init()
    {
        UIManager uiManager = GenericSingleton<UIManager>.Instance;
        uiManager.UI = this;
        uiManager.EventUI = _eventUI;
        uiManager.GameOverUI = _gameOverUI;
    }

    void GameStart()
    {
        StartBGM();
        GameManager gameManager = GenericSingleton<GameManager>.Instance;
        gameManager.GameState = EGameStateType.Move;
        gameManager.IsGameStart = true;
    }

    void StartBGM()
    {
        SoundManager soundManager = GenericSingleton<SoundManager>.Instance;
        AudioClip bgmAudio = Resources.Load("Prefabs/AudioClip/BGM") as AudioClip;
        soundManager.Init();
        soundManager.StartBGM(bgmAudio);
    }

    public void ShowScore()
    {
        int score = GenericSingleton<ScoreManager>.Instance.Score;
        _scoreText.text = score.ToString();
    }

    public void ShowRemainTime()
    {
        GameManager gameManager = GenericSingleton<GameManager>.Instance;
        float currentTime = gameManager.CurrentTime;
        float maxTime = gameManager.MaxTime;
        _timeBar.fillAmount = currentTime / maxTime;
        _timeText.text = string.Format("{0:0.#}", currentTime);
    }

    public void Stop()
    {
        _animator.SetBool("isStop", true);
        _soundManager.PlaySFX(_buttonAudio);
        _gameManager.GameState = EGameStateType.Pause;
    }

    public void GameOver()
    {
        _animator.SetBool("isGameOver", true);
    }
}