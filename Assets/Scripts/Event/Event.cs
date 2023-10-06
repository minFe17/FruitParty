using UnityEngine;
using Utils;

public abstract class Event : MonoBehaviour
{
    protected EEventType _eventType;
    protected GameManager _gameManager;
    protected TileManager _tileManager;
    protected int _width;
    protected int _height;

    protected virtual void Start()
    {
        _gameManager = GenericSingleton<GameManager>.Instance;
        _tileManager = GenericSingleton<TileManager>.Instance;
    }

    public abstract void EventEffect();
}