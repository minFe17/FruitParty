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

    [Header("GameStopUI")]
    [SerializeField] GameObject _stopUI;

    [Header("GameOverUI")]
    [SerializeField] GameObject _gameOverUI;

    EventUIManager _eventUIManager;
    GameManager _gameManager;
    SoundManager _soundManager;
    AudioClipManager _audioClipManager;
    Animator _animator;

    public Animator UIAnimator { get => _animator; }

    void Start()
    {
        _eventUIManager = GenericSingleton<EventUIManager>.Instance;
        _gameManager = GenericSingleton<GameManager>.Instance;
        _soundManager = GenericSingleton<SoundManager>.Instance;
        _audioClipManager = GenericSingleton<AudioClipManager>.Instance;
        _animator = GetComponent<Animator>();
    }

    public void Init()
    {
        UIManager uiManager = GenericSingleton<UIManager>.Instance;
        uiManager.UI = this;
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
        soundManager.Init();
        soundManager.StartBGM(_audioClipManager.InGameBGM);
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
        _soundManager.PlaySFX(_audioClipManager.ButtonSfX);
        _gameManager.GameState = EGameStateType.Pause;
    }

    public void GameOver()
    {
        _gameOverUI.SetActive(true);
        _animator.SetTrigger("doGameOver");
    }

    public void InitEventUI(EEventType eventType)
    {
        EventUIBase _eventUI;
        _eventUIManager.EventUI.TryGetValue(eventType, out _eventUI);
        _eventUI.InitEventUI();
    }
}