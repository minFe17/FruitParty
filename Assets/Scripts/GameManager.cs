using UnityEngine;

public class GameManager : MonoBehaviour
{
    // �̱���
    public EGameStateType GameState { get; set; }
}

public enum EGameStateType
{
    Move,
    Wait,
}