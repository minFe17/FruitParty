using UnityEngine;
using Utils;

public class UI : MonoBehaviour
{
    [Header("GameUI Panel")]
    [SerializeField] GameUIPanel _gameUIPanel;

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

    void Awake()
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
        uiManager.GameUIPanel = _gameUIPanel;
    }

    void GameStart()
    {
        StartBGM();
        _gameManager.ChangeGameState(EGameStateType.Move);
        _gameManager.IsGameStart = true;
    }

    void StartBGM()
    {
        _soundManager.CreateSoundController();
        _audioClipManager.PlayBGM();
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