using UnityEngine;
using Utils;

public class GameManager : MonoBehaviour
{
    // ╫л╠шео
    UIManager _uiManager;
    EGameStateType _gameState;

    float _time;
    float _currentTime;
    float _addTimeAmount = 1f;
    float _maxTime = 100f;
    bool _isGameStart;

    public EGameStateType GameState { get => _gameState; set => _gameState = value; }
    public float CurrentTime { get => _currentTime; }
    public float MaxTime { get => _maxTime; }
    public bool IsGameStart { set => _isGameStart = value; }

    void Update()
    {
        CheckTime();
    }

    public void Init()
    {
        _uiManager = GenericSingleton<UIManager>.Instance;
        _gameState = EGameStateType.Wait;
        _time = 0f;
        _currentTime = 30f;
        _isGameStart = false;
        _uiManager.GameUIPanel.ShowRemainTime();
    }

    void CheckTime()
    {
        if (_gameState == EGameStateType.Move || _gameState == EGameStateType.Wait)
        {
            if (_isGameStart)
            {
                _time += Time.deltaTime;
                if (_time >= 1f)
                {
                    _currentTime -= 1f;
                    _time = 0f;
                    _uiManager.GameUIPanel.ShowRemainTime();
                }
                if (_currentTime <= 0)
                    GameOver();
            }
        }
    }

    void GameOver()
    {
        _gameState = EGameStateType.GameOver;
        _uiManager.UI.GameOver();
    }

    public void AddTime(int streak)
    {
        _currentTime = _currentTime + (_addTimeAmount * streak);
        if (_currentTime >= _maxTime)
            _currentTime = _maxTime;
        _uiManager.GameUIPanel.ShowRemainTime();
    }
}