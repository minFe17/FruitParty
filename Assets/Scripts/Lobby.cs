using UnityEngine;

public class Lobby : MonoBehaviour
{
    [SerializeField] Camera _mainCamera;

    void Start()
    {
        CreateLobbyUI();
    }

    void CreateLobbyUI()
    {
        GameObject temp = Resources.Load("Prefabs/UI/LobbyUI") as GameObject;
        GameObject lobbyUI = Instantiate(temp);
        lobbyUI.GetComponent<Canvas>().worldCamera = _mainCamera;
    }
}