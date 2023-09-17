using UnityEngine;
using Utils;

public class GameManager : MonoBehaviour
{
    // 싱글턴
    UIManager _uiManager;
    EGameStateType _gameState;

    float _time;
    float _currentTime;
    float _addTimeAmount;
    float _maxTime = 100f;
    bool _isGameStart;

    public EGameStateType GameState { get => _gameState; set => _gameState = value; }
    public float CurrentTime { get => _currentTime; }
    public float MaxTime { get => _maxTime; }
    public bool IsGameStart { set => _isGameStart = value; }

    void Update()
    {
        if (_isGameStart == true)
            CheckTime();
    }

    public void Init()
    {
        _uiManager = GenericSingleton<UIManager>.Instance;
        _gameState = EGameStateType.Wait;
        _time = 0f;
        _currentTime = 30f;
        _isGameStart = false;
        _uiManager.UI.ShowRemainTime();
    }

    void CheckTime()
    {
        _time += Time.deltaTime;
        if (_time >= 1f)
        {
            _currentTime -= 1f;
            _time = 0f;
            _uiManager.UI.ShowRemainTime();
        }
        
        if (_currentTime <= 0)
            GameOver();
    }

    void AddTime()
    {
        _currentTime += _addTimeAmount;
        if (_currentTime >= _maxTime)
            _currentTime = _maxTime;
        _uiManager.UI.ShowRemainTime();
    }

    void GameOver()
    {
        // 게임오버 UI 띄우기
        // 게임오버 UI는 화면이 어두워지고 위에서 아래로 UI 내려오기
    }
}

public enum EGameStateType
{
    Move,
    Wait,
}