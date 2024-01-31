using UnityEngine;
using Utils;

public class Board : MonoBehaviour
{
    [Header("Board Size")]
    [SerializeField] int _width;
    [SerializeField] int _height;
    [SerializeField] int _offset;

    FruitManager _fruitManager;
    TileManager _tileManager;

    void Start()
    {
        Init(); 
        CreateBoard();
    }

    void Init()
    {
        GenericSingleton<ScoreManager>.Instance.SetScore();
        GenericSingleton<EventUIManager>.Instance.Init();
        GenericSingleton<GameManager>.Instance.Init();
        _fruitManager = GenericSingleton<FruitManager>.Instance;
        _tileManager = GenericSingleton<TileManager>.Instance;
        _fruitManager.Init(_width, _height);
        _fruitManager.Offset = _offset;
        _tileManager.Init(_width, _height);
        GenericSingleton<EventManager>.Instance.Init();
    }

    void CreateBoard()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                Vector2Int position = new Vector2Int(i, j + _offset);
                Transform tilePos = _tileManager.CreateNormalTile(i, j);
                _fruitManager.CreateFruit(tilePos, position);
            }
        }
        GenericSingleton<HintManager>.Instance.Init();
    }
}