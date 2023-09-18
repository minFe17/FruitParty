using UnityEngine;

public class Lobby : MonoBehaviour
{
    void Start()
    {
        CreateLobby();
    }

    void CreateLobby()
    {
        CreateCamera();
        CreateLobbyUI();
    }

    void CreateCamera()
    {
        GameObject temp = Resources.Load("Prefabs/Main Camera") as GameObject;
        GameObject camera = Instantiate(temp);
        CreateBackground(camera.GetComponent<Camera>());
    }

    void CreateBackground(Camera mainCamera)
    {
        GameObject temp = Resources.Load("Prefabs/Background") as GameObject;
        GameObject background = Instantiate(temp);
        background.GetComponent<Canvas>().worldCamera = mainCamera;
    }

    void CreateLobbyUI()
    {
        GameObject temp = Resources.Load("Prefabs/UI/LobbyUI") as GameObject;
        Instantiate(temp);
    }
}