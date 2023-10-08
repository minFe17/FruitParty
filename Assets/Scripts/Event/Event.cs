using UnityEngine;
using Utils;

public abstract class Event : MonoBehaviour
{
    protected EEventType _eventType;
    protected FruitManager _fruitManager;
    protected GameManager _gameManager;
    protected TileManager _tileManager;
    protected int _width;
    protected int _height;
    protected int _maxCreatableTiles = 10;

    protected virtual void Start()
    {
        _fruitManager = GenericSingleton<FruitManager>.Instance;
        _gameManager = GenericSingleton<GameManager>.Instance;
        _tileManager = GenericSingleton<TileManager>.Instance;
    }

    public abstract void EventEffect();
}