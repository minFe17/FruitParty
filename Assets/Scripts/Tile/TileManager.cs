using System.Collections.Generic;
using UnityEngine;
using Utils;

public class TileManager : MonoBehaviour
{
    // ╫л╠шео
    List<TileType> _boardLayout = new List<TileType>();
    GameObject _tiles;
    FruitManager _fruitManager;

    [Header("Tile Array")]
    GameObject[,] _allTiles;
    bool[,] _blankTiles;
    IceTile[,] _iceTiles;
    LockTile[,] _lockTiles;
    ConcreteTile[,] _concreteTiles;
    LavaTile[,] _lavaTiles;

    [Header("Tile Prefab")]
    GameObject _tilePrefab;
    GameObject _iceTilePrefab;
    GameObject _lockTilePrefab;
    GameObject _concreteTilePrefab;
    GameObject _lavaTilePrefab;

    int _width;
    int _height;
    bool _createMoreLavaTile = false;

    public bool[,] BlankTiles { get => _blankTiles; }
    public IceTile[,] IceTiles { get => _iceTiles; }
    public LockTile[,] LockTiles { get => _lockTiles; }
    public ConcreteTile[,] ConcreteTiles { get => _concreteTiles; }
    public LavaTile[,] LavaTiles { get => _lavaTiles; }
    public bool CreateMoreLavaTile { get => _createMoreLavaTile; set => _createMoreLavaTile = value; }

    public void Init(int width, int height)
    {
        _width = width;
        _height = height;
        SetArray();
        LoadTilePrefab();
        _tiles = new GameObject("Tiles");
        _fruitManager = GenericSingleton<FruitManager>.Instance;
    }

    void SetArray()
    {
        _allTiles = new GameObject[_width, _height];
        _blankTiles = new bool[_width, _height];
        _iceTiles = new IceTile[_width, _height];
        _lockTiles = new LockTile[_width, _height];
        _concreteTiles = new ConcreteTile[_width, _height];
        _lavaTiles = new LavaTile[_width, _height];
    }

    void LoadTilePrefab()
    {
        _tilePrefab = Resources.Load("Prefabs/Tile/Tile") as GameObject;
        _iceTilePrefab = Resources.Load("Prefabs/Tile/IceTile") as GameObject;
        _lockTilePrefab = Resources.Load("Prefabs/Tile/LockTile") as GameObject;
        _concreteTilePrefab = Resources.Load("Prefabs/Tile/ConcreteTile") as GameObject;
        _lavaTilePrefab = Resources.Load("Prefabs/Tile/LavaTile") as GameObject;
    }

    void GenerateTiles()
    {
        for (int i = 0; i < _boardLayout.Count; i++)
        {
            switch (_boardLayout[i].TileKindType)
            {
                case ETileKindType.Blank:
                    GenerateBlankTiles(_boardLayout[i].X, _boardLayout[i].Y);
                    break;
                case ETileKindType.Ice:
                    GenerateIceTiles(_boardLayout[i].X, _boardLayout[i].Y);
                    break;
                case ETileKindType.Lock:
                    GenerateLockTiles(_boardLayout[i].X, _boardLayout[i].Y);
                    break;
                case ETileKindType.Concrete:
                    GenerateConcreteTiles(_boardLayout[i].X, _boardLayout[i].Y);
                    break;
                case ETileKindType.Lava:
                    GenerateLavaTiles(_boardLayout[i].X, _boardLayout[i].Y);
                    break;
            }
        }
    }

    void GenerateBlankTiles(int column, int row)
    {
        _blankTiles[column, row] = true;
        if (_allTiles[column, row] != null)
        {
            Destroy(_allTiles[column, row]);
            _allTiles[column, row] = null;
        }
    }

    void GenerateIceTiles(int column, int row)
    {
        GameObject iceTile = CreateTiles(column, row, _iceTilePrefab);
        _iceTiles[column, row] = iceTile.GetComponent<IceTile>();
    }

    void GenerateLockTiles(int column, int row)
    {
        GameObject lockTile = CreateTiles(column, row, _lockTilePrefab);
        _lockTiles[column, row] = lockTile.GetComponent<LockTile>();
    }

    void GenerateConcreteTiles(int column, int row)
    {
        CheckFruit(column, row);
        GameObject concreteTile = CreateTiles(column, row, _concreteTilePrefab);
        _concreteTiles[column, row] = concreteTile.GetComponent<ConcreteTile>();
    }

    void GenerateLavaTiles(int column, int row)
    {
        CheckFruit(column, row);
        GameObject lavaTile = CreateTiles(column, row, _lavaTilePrefab);
        _lavaTiles[column, row] = lavaTile.GetComponent<LavaTile>();
    }

    void CheckFruit(int column, int row)
    {
        if (_fruitManager.AllFruits[column, row] != null)
        {
            Destroy(_fruitManager.AllFruits[column, row]);
            _fruitManager.AllFruits[column, row] = null;
        }
    }

    GameObject CreateTiles(int column, int row, GameObject prefab)
    {
        Vector2 position = new Vector2(column, row);
        GameObject tile = Instantiate(prefab, position, Quaternion.identity);
        tile.GetComponent<Tile>().Init(column, row);
        return tile;
    }

    void CreateMoreLavaTiles()
    {
        bool makeLavaTile = false;
        int iterations = 0;
        while (!makeLavaTile && iterations < 200)
        {
            int column = Random.Range(0, _width);
            int row = Random.Range(0, _height);
            if (_lavaTiles[column, row] != null)
            {
                Vector2Int direction = CheckForDirection(column, row);
                if (direction != Vector2Int.zero)
                {
                    int newColumn = column + direction.x;
                    int newRow = row + direction.y;
                    GenerateLavaTiles(newColumn, newRow);
                    makeLavaTile = true;
                }
            }
            iterations++;
        }
    }

    Vector2Int CheckForDirection(int column, int row)
    {
        if (_fruitManager.AllFruits[column + 1, row] && column < _width - 1)
            return Vector2Int.right;
        if (_fruitManager.AllFruits[column - 1, row] && column > 0)
            return Vector2Int.left;
        if (_fruitManager.AllFruits[column, row + 1] && row < _height - 1)
            return Vector2Int.up;
        if (_fruitManager.AllFruits[column, row - 1] && row > 0)
            return Vector2Int.down;

        return Vector2Int.zero;
    }

    public Transform CreateTile(int width, int height)
    {
        Vector2 position = new Vector2(width, height);
        GameObject tile = Instantiate(_tilePrefab, position, Quaternion.identity);
        tile.transform.parent = _tiles.transform;
        tile.name = $"Tile ({width}, {height})";
        _allTiles[width, height] = tile;
        return tile.transform;
    }

    public void CheckCreateMoreLavaTiles()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_lavaTiles[i, j] != null && _createMoreLavaTile)
                {
                    CreateMoreLavaTiles();
                }
            }
        }
    }
}