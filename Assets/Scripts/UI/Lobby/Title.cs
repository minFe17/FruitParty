using UnityEngine;

public class Title : MonoBehaviour
{
    [SerializeField] LobbyUI _parent;

    void EndMoveTitle()
    {
        _parent.ShowScoreAndPlayButton();
    }
}