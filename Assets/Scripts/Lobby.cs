using UnityEngine;
using Utils;

public class Lobby : MonoBehaviour
{
    [SerializeField] Camera _mainCamera;

    async void Start()
    {
        await GenericSingleton<LoadAsset>.Instance.Init();
        CreateLobbyUI();
        CreateSoundController();
    }

    void CreateLobbyUI()
    {
        GenericSingleton<UIManager>.Instance.CreateLobbyUI(_mainCamera);
    }

    void CreateSoundController()
    {
        GenericSingleton<SoundManager>.Instance.CreateSoundController();
    }
}