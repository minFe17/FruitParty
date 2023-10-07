using System.Collections.Generic;
using UnityEngine;
using Utils;

public class TileManager : MonoBehaviour
{
    // ╫л╠шео
    List<TileType> _boardLayout = new List<TileType>();
    GameObject _tileParent;
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

    public List<TileType> BoardLayout { get => _boardLayout; }
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
        _tileParent = new GameObject("Tiles");
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

                    CheckFruit(column, row);
                    GameObject lavaTile = CreateTiles(column, row, _lavaTilePrefab);
                    _lavaTiles[column, row] = lavaTile.GetComponent<LavaTile>();
                    makeLavaTile = true;
                }
            }
            iterations++;
        }
    }

    void CalculateHaveFruitTile(out int column, out int row, ETileKindType type)
    {
        int tileX = 0;
        int tileY = 0;
        column = 0;
        row = 0;
        bool canCreateTilePosition = false;
        do
        {
            column = Random.Range(0, _width);
            row = Random.Range(0, _height);
            for (int i = 0; i < _boardLayout.Count; i++)
            {
                tileX = _boardLayout[i].X;
                tileY = _boardLayout[i].Y;
                if (tileX == column && tileY == row)
                {
                    ETileKindType tileType = _boardLayout[i].TileKindType;
                    if (tileType == ETileKindType.Ice && type == ETileKindType.Lock)
                    {
                        canCreateTilePosition = true;
                        break;
                    }
                    else if (tileType == ETileKindType.Lock && type == ETileKindType.Ice)
                    {
                        canCreateTilePosition = true;
                        break;
                    }
                    else
                    {
                        canCreateTilePosition = false;
                        break;
                    }
                }
                else
                    canCreateTilePosition = true;
            }
        }
        while (!canCreateTilePosition);
    }

    void CalculateObstacleTile(out int column, out int row)
    {
        int tileX = 0;
        int tileY = 0;
        column = 0;
        row = 0;
        bool canCreateTilePosition = false;
        do
        {
            column = Random.Range(0, _width);
            row = Random.Range(0, _height);
            for (int i = 0; i < _boardLayout.Count; i++)
            {
                tileX = _boardLayout[i].X;
                tileY = _boardLayout[i].Y;
                if (tileX == column && tileY == row)
                {
                    canCreateTilePosition = false;
                    break;
                }
                else
                    canCreateTilePosition = true;
            }
        }
        while (!canCreateTilePosition);
    }

    void AddBoardLayout(ETileKindType type, Tile tile, int column, int row)
    {
        TileType tileType = new TileType(type, tile, column, row);
        _boardLayout.Add(tileType);
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

    public Transform CreateTile(int column, int row)
    {
        Vector2 position = new Vector2(column, row);
        GameObject tile = Instantiate(_tilePrefab, position, Quaternion.identity);
        tile.transform.parent = _tileParent.transform;
        tile.name = $"Tile ({column}, {row})";
        _allTiles[column, row] = tile;
        return tile.transform;
    }

    public void CreateBlankTiles()
    {
        int column;
        int row;
        CalculateObstacleTile(out column, out row);
        _blankTiles[column, row] = true;
        if (_allTiles[column, row] != null)
        {
            Destroy(_allTiles[column, row]);
            _allTiles[column, row] = null;
        }
        AddBoardLayout(ETileKindType.Blank, null, column, row);
    }

    public void CreateIceTiles()
    {
        int column;
        int row;
        CalculateHaveFruitTile(out column, out row, ETileKindType.Ice);
        GameObject tile = CreateTiles(column, row, _iceTilePrefab);
        IceTile iceTile = tile.GetComponent<IceTile>();
        _iceTiles[column, row] = iceTile;
        AddBoardLayout(ETileKindType.Ice, iceTile, column, row);
    }

    public void CreateLockTiles()
    {
        int column;
        int row;
        CalculateHaveFruitTile(out column, out row, ETileKindType.Lock);
        GameObject tile = CreateTiles(column, row, _lockTilePrefab);
        LockTile lockTile = tile.GetComponent<LockTile>();
        _lockTiles[column, row] = lockTile;
        AddBoardLayout(ETileKindType.Lock, lockTile, column, row);
    }

    public void CreateConcreteTiles()
    {
        int column;
        int row;
        CalculateObstacleTile(out column, out row);
        CheckFruit(column, row);
        GameObject tile = CreateTiles(column, row, _concreteTilePrefab);
        ConcreteTile concreteTile = tile.GetComponent<ConcreteTile>();
        _concreteTiles[column, row] = concreteTile;
        AddBoardLayout(ETileKindType.Concrete, concreteTile, column, row);
    }

    public void CreateLavaTiles()
    {
        int column;
        int row;
        CalculateObstacleTile(out column, out row);
        CheckFruit(column, row);
        GameObject tile = CreateTiles(column, row, _lavaTilePrefab);
        LavaTile lavaTile = tile.GetComponent<LavaTile>();
        _lavaTiles[column, row] = lavaTile;
        AddBoardLayout(ETileKindType.Lava, lavaTile, column, row);
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

    public void DestroyBoardLayout(Tile tile)
    {
        _boardLayout.Remove(tile.TileType);
    }

    public void ResetTile()
    {
        for (int i = 0; i < _boardLayout.Count; i++)
        {
            if (_boardLayout[i].TileKindType == ETileKindType.Blank)
                CreateTile(_boardLayout[i].X, _boardLayout[i].Y);
            else
                _boardLayout[i].Tile.DestroyTile();
        }
        _boardLayout.Clear();
    }
}