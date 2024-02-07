using UnityEngine;
using Utils;

public class Groundwork : MonoBehaviour
{
    [SerializeField] Camera _mainCamera;

    void Start()
    {
        CreateUI();
        Init();
    }

    void CreateUI()
    {
        GenericSingleton<UIManager>.Instance.CreateUI(_mainCamera);
    }

    void Init()
    {
        GenericSingleton<ScoreManager>.Instance.SetScore();
        GenericSingleton<EventUIManager>.Instance.Init();
        GenericSingleton<GameManager>.Instance.Init();
        GenericSingleton<EventManager>.Instance.Init();
        GenericSingleton<BombManager>.Instance.Init();
    }
}