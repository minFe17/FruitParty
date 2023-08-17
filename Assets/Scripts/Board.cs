using UnityEngine;
using Utils;

public class Board : MonoBehaviour
{
    [SerializeField] int _width;
    [SerializeField] int _height;
    [SerializeField] int _offset;

    Tile[,] _allTiles;
    GameObject _tilePrefab;
    FruitManager _fruitManager;

    void Start()
    {
        _allTiles = new Tile[_width, _height];
        _tilePrefab = Resources.Load("Prefabs/Tile") as GameObject;
        _fruitManager = GenericSingleton<FruitManager>.Instance;
        _fruitManager.Init(_width, _height);
        _fruitManager.Offset = _offset;
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
        return tile.transform;
    }
}
