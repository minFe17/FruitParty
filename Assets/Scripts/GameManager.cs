using UnityEngine;

public class GameManager : MonoBehaviour
{
    // ╫л╠шео
    EGameStateType _gameState;
    bool _isGameStart;

    public EGameStateType GameState { get => _gameState; set => _gameState = value; }
    public bool IsGameStart { set => _isGameStart = value; }

    public void Init()
    {
        _gameState = EGameStateType.Wait;
        _isGameStart = false;
    }
}

public enum EGameStateType
{
    Move,
    Wait,
}