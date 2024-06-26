using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using Utils;

public class GameUIPanel : MonoBehaviour
{
    [SerializeField] Animator _animator;

    [Header("TimeUI")]
    [SerializeField] Image _timeBar;
    [SerializeField] Text _timeText;

    [Header("ScoreUI")]
    [SerializeField] Text _scoreText;

    [Header("Sprite")]
    [SerializeField] Image _timeBarBaseSprite;
    [SerializeField] Image _stopButtonSprite;

    SpriteAtlas _uiAtlas;
    GameManager _gameManager;
    SoundManager _soundManager;
    AudioClipManager _audioClipManager;

    void Awake()
    {
        _uiAtlas = GenericSingleton<SpriteManager>.Instance.UIAtlas;
        _gameManager = GenericSingleton<GameManager>.Instance;
        _soundManager = GenericSingleton<SoundManager>.Instance;
        _audioClipManager = GenericSingleton<AudioClipManager>.Instance;

        SetSprite();
    }

    void SetSprite()
    {
        _timeBar.sprite = _uiAtlas.GetSprite("Time Bar");
        _timeBarBaseSprite.sprite = _uiAtlas.GetSprite("Time Bar Base");
        _stopButtonSprite.sprite = _uiAtlas.GetSprite("Stop Button");
    }

    public void ShowScore()
    {
        int score = GenericSingleton<ScoreManager>.Instance.Score;
        _scoreText.text = score.ToString();
    }

    public void ShowRemainTime()
    {
        float currentTime = _gameManager.CurrentTime;
        float maxTime = _gameManager.MaxTime;
        _timeBar.fillAmount = currentTime / maxTime;
        _timeText.text = string.Format("{0:0.#}", currentTime);
    }

    public void Stop()
    {
        _animator.SetBool("isStop", true);
        _soundManager.PlaySFX(_audioClipManager.ButtonSfX);
        _gameManager.ChangeGameState(EGameStateType.Pause);
    }
}