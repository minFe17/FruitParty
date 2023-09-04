using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Board : MonoBehaviour
{
    [SerializeField] int _width;
    [SerializeField] int _height;
    [SerializeField] int _offset;

    List<TileType> _boardLayout = new List<TileType>();
    GameObject[,] _allTiles;
    bool[,] _blankSpaces;

    GameObject _cameraPrefab;
    GameObject _tilePrefab;
    FruitManager _fruitManager;

    void Start()
    {
        _allTiles = new GameObject[_width, _height];
        _blankSpaces = new bool[_width, _height];
        _cameraPrefab = Resources.Load("Prefabs/Main Camera") as GameObject;
        _tilePrefab = Resources.Load("Prefabs/Tile") as GameObject;
        _fruitManager = GenericSingleton<FruitManager>.Instance;
        _fruitManager.Init(_width, _height);
        _fruitManager.Offset = _offset;
        CreateCamera();
        Init();
    }

    void Init()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                Vector2Int position = new Vector2Int(i, j + _offset);
                Transform tilePos = CreateTile(position);
                _fruitManager.CreateFruit(tilePos, position);
            }
        }
        GenericSingleton<GameManager>.Instance.GameState = EGameStateType.Move;
    }

    Transform CreateTile(Vector2 position)
    {
        GameObject tile = Instantiate(_tilePrefab, position, Quaternion.identity);
        tile.transform.parent = this.transform;
        tile.name = $"Tile ({position.x}, {position.y})";
        _allTiles[(int)position.x, (int)position.y] = tile;
        return tile.transform;
    }

    //지진 효과할 때 사용
    void GenerateBlankSpaces()
    {
        for (int i = 0; i < _boardLayout.Count; i++)
        {
            if (_boardLayout[i].TileKindType == ETileKindType.Blank)
            {
                int x = _boardLayout[i].X;
                int y = _boardLayout[i].Y;
                _blankSpaces[x,y] = true;
                if (_allTiles[x, y] != null)
                {
                    Destroy(_allTiles[x, y]);
                    _allTiles[x, y] = null;
                }
            }
        }
    }

    void CreateCamera()
    {
        GameObject mainCamera = Instantiate(_cameraPrefab);
        mainCamera.GetComponent<CameraScalar>().SettingCameraPosition(_width, _height);
    }
}