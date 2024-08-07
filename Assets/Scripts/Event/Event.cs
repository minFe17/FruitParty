using UnityEngine;
using Utils;

public class Event : MonoBehaviour
{
    protected EEventType _eventType;
    protected EventUIBase _eventUI;

    protected EventManager _eventManager;
    protected EventUIManager _eventUIManager;
    protected FruitManager _fruitManager;
    protected GameManager _gameManager;
    protected TileManager _tileManager;

    protected int _width;
    protected int _height;
    protected int _minCreatableTiles = 5;
    protected int _maxCreatableTiles = 10;

    protected float _eventDelay = 0.5f;
    protected float _eventUIDelay = 1f;

    protected virtual void Start()
    {
        _eventManager = GenericSingleton<EventManager>.Instance;
        _eventUIManager = GenericSingleton<EventUIManager>.Instance;
        _fruitManager = GenericSingleton<FruitManager>.Instance;
        _gameManager = GenericSingleton<GameManager>.Instance;
        _tileManager = GenericSingleton<TileManager>.Instance;

        _width = _fruitManager.Width;
        _height = _fruitManager.Height;
    }
}