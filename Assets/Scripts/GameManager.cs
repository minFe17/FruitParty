using UnityEngine;

public class GameManager : MonoBehaviour
{
    // ╫л╠шео
    public EGameStateType GameState { get; set; }
}

public enum EGameStateType
{
    Move,
    Wait,
}