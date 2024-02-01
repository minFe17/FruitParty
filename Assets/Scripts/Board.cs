using UnityEngine;
using Utils;

public class Board : MonoBehaviour
{
    [SerializeField] CameraScalar _camera;

    [Header("Board Size")]
    [SerializeField] int _width;
    [SerializeField] int _height;
    [SerializeField] int _offset;

    FruitManager _fruitManager;
    TileManager _tileManager;

    void Start()
    {
        SetCamera();
        Init();
        CreateBoard();
    }

    void Init()
    {
        _fruitManager = GenericSingleton<FruitManager>.Instance;
        _tileManager = GenericSingleton<TileManager>.Instance;
        _fruitManager.Init(_width, _height);
        _fruitManager.Offset = _offset;
        _tileManager.Init(_width, _height);
    }

    void SetCamera()
    {
        _camera.SettingCameraPosition(_width, _height);
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