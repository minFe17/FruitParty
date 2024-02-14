using System.Collections.Generic;
using UnityEngine;
using Utils;

public class TileManager : MonoBehaviour
{
    // ╫л╠шео
    public List<TileType> _boardLayout = new List<TileType>();
    GameObject _tileParent;
    FruitManager _fruitManager;
    FactoryManager _factoryManager;

    [Header("Tile Array")]
    GameObject[,] _allTiles;
    bool[,] _blankTiles;
    Tile[,] _iceTiles;
    Tile[,] _lockTiles;
    Tile[,] _concreteTiles;
    Tile[,] _lavaTiles;

    int _width;
    int _height;
    int _column;
    int _row;
    bool _createMoreLavaTile = false;

    public bool[,] BlankTiles { get => _blankTiles; }
    public Tile[,] IceTiles { get => _iceTiles; }
    public Tile[,] LockTiles { get => _lockTiles; }
    public Tile[,] ConcreteTiles { get => _concreteTiles; }
    public Tile[,] LavaTiles { get => _lavaTiles; }
    public bool CreateMoreLavaTile { get => _createMoreLavaTile; set => _createMoreLavaTile = value; }
    public bool FirstCreateLavaTile { get; set; }

    public void Init(int width, int height)
    {
        _width = width;
        _height = height;
        SetArray();
        _tileParent = new GameObject("Tiles");
        _fruitManager = GenericSingleton<FruitManager>.Instance;
        _factoryManager = GenericSingleton<FactoryManager>.Instance;
        _boardLayout.Clear();
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

    void CheckFruit()
    {
        if (_fruitManager.AllFruits[_column, _row] != null)
            _fruitManager.DestroyFruit(_fruitManager.AllFruits[_column, _row]);
    }

    Tile CreateTiles(ETileKindType type)
    {
        TileType tileType;
        Vector2Int position = new Vector2Int(_column, _row);
        Tile tile = _factoryManager.MakeObject<ETileKindType, Tile>(type, position);
        AddBoardLayout(out tileType, type, tile);
        tile.Init(tileType, _column, _row);
        return tile;
    }

    void CalculateHaveFruitTile(ETileKindType type)
    {
        int tileX = 0;
        int tileY = 0;
        bool canCreateTilePosition = true;
        do
        {
            _column = Random.Range(0, _width);
            _row = Random.Range(0, _height);
            for (int i = 0; i < _boardLayout.Count; i++)
            {
                tileX = _boardLayout[i].X;
                tileY = _boardLayout[i].Y;
                if (tileX == _column && tileY == _row)
                {
                    ETileKindType tileType = _boardLayout[i].TileKindType;
                    if ((tileType == ETileKindType.Ice && type == ETileKindType.Lock) ||
                        (tileType == ETileKindType.Lock && type == ETileKindType.Ice))
                    {
                        canCreateTilePosition = true;
                    }
                    else
                    {
                        canCreateTilePosition = false;
                        break;
                    }
                }
                else
                {
                    canCreateTilePosition = true;
                }
            }
        }
        while (!canCreateTilePosition);
    }

    void CalculateObstacleTile()
    {
        int tileX = 0;
        int tileY = 0;
        bool canCreateTilePosition = true;
        do
        {
            _column = Random.Range(0, _width);
            _row = Random.Range(0, _height);
            for (int i = 0; i < _boardLayout.Count; i++)
            {
                tileX = _boardLayout[i].X;
                tileY = _boardLayout[i].Y;
                if (tileX == _column && tileY == _row)
                {
                    canCreateTilePosition = false;
                    break;
                }
                else
                {
                    canCreateTilePosition = true;
                }
            }
        }
        while (!canCreateTilePosition);
    }

    void AddBoardLayout(out TileType tileType, ETileKindType type, Tile tile)
    {
        tileType = new TileType(type, tile, _column, _row);
        _boardLayout.Add(tileType);
    }

    Vector2Int CheckForDirection(int column, int row)
    {
        if (column < _width - 1 && _fruitManager.AllFruits[column + 1, row])
            return Vector2Int.right;
        if (column > 0 && _fruitManager.AllFruits[column - 1, row])
            return Vector2Int.left;
        if (row < _height - 1 && _fruitManager.AllFruits[column, row + 1])
            return Vector2Int.up;
        if (row > 0 && _fruitManager.AllFruits[column, row - 1])
            return Vector2Int.down;

        return Vector2Int.zero;
    }

    void CheckHaveFruitTile(int column, int row)
    {
        if (_lockTiles[column, row] != null)
        {
            DestroyTile(_lockTiles[column, row]);
            _fruitManager.StreakValue--;
            return;
        }
        if (_iceTiles[column, row] != null)
        {
            _iceTiles[column, row].TakeDamage();
            _fruitManager.StreakValue--;
            return;
        }
    }

    void CheckHitTile(int column, int row)
    {
        if (column > 0)
        {
            CheckConcreteTile(column - 1, row);
            CheckLavaTile(column - 1, row);
        }
        if (column < _width - 1)
        {
            CheckConcreteTile(column + 1, row);
            CheckLavaTile(column + 1, row);
        }

        if (row > 0)
        {
            CheckConcreteTile(column, row - 1);
            CheckLavaTile(column, row - 1);
        }
        if (row < _height - 1)
        {
            CheckConcreteTile(column, row + 1);
            CheckLavaTile(column, row + 1);
        }
    }

    void CheckConcreteTile(int column, int row)
    {
        if (_concreteTiles[column, row])
            DestroyTile(_concreteTiles[column, row]);
    }

    void CheckLavaTile(int column, int row)
    {
        if (_lavaTiles[column, row])
        {
            DestroyTile(_lavaTiles[column, row]);
            _createMoreLavaTile = false;
        }
    }

    public void CheckTile(int column, int row)
    {
        CheckHaveFruitTile(column, row);
        CheckHitTile(column, row);
    }

    public void CreateNormalTile(int column, int row)
    {
        Vector2Int position = new Vector2Int(column, row);
        Tile tile = _factoryManager.MakeObject<ETileKindType, Tile>(ETileKindType.Normal, position);
        tile.transform.parent = _tileParent.transform;
        _allTiles[column, row] = tile.gameObject;
    }

    public void CreateBlankTiles()
    {
        TileType tileType;
        CalculateObstacleTile();
        AddBoardLayout(out tileType, ETileKindType.Blank, null);
        _blankTiles[_column, _row] = true;
        if (_allTiles[_column, _row] != null)
        {
            _allTiles[_column, _row].GetComponent<Tile>().DestroyTile();
            _fruitManager.DestroyFruit(_fruitManager.AllFruits[_column, _row]);
            CheckFruit();
        }
    }

    public void CreateIceTiles()
    {
        CalculateHaveFruitTile(ETileKindType.Ice);
        Tile tile = CreateTiles(ETileKindType.Ice);
        _iceTiles[_column, _row] = tile;
    }

    public void CreateLockTiles()
    {
        CalculateHaveFruitTile(ETileKindType.Lock);
        Tile tile = CreateTiles(ETileKindType.Lock);
        _lockTiles[_column, _row] = tile;
    }

    public void CreateConcreteTiles()
    {
        CalculateObstacleTile();
        CheckFruit();
        Tile tile = CreateTiles(ETileKindType.Concrete);
        _concreteTiles[_column, _row] = tile;
    }

    public void CreateLavaTiles()
    {
        CalculateObstacleTile();
        CheckFruit();
        Tile tile = CreateTiles(ETileKindType.Lava);
        _lavaTiles[_column, _row] = tile;
    }

    public bool LavaTileInBoard()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_lavaTiles[i, j] != null)
                    return true;
            }
        }
        return false;
    }

    public void CheckCreateMoreLavaTile()
    {
        if (LavaTileInBoard() && !FirstCreateLavaTile)
            _createMoreLavaTile = true;
        if (FirstCreateLavaTile)
            FirstCreateLavaTile = false;
    }

    public void CreateMoreLavaTiles()
    {
        if (_createMoreLavaTile)
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
                        _column = column + direction.x;
                        _row = row + direction.y;

                        CheckFruit();
                        Tile lavaTile = CreateTiles(ETileKindType.Lava);
                        _lavaTiles[_column, _row] = lavaTile;
                        makeLavaTile = true;
                    }
                }
                iterations++;
            }
        }
    }

    public void DestroyTile(Tile tile)
    {
        _boardLayout.Remove(tile.TileType);
        tile.DestroyTile();
    }

    public void ResetTile()
    {
        for (int i = 0; i < _boardLayout.Count; i++)
        {
            if (_boardLayout[i].TileKindType == ETileKindType.Blank)
                CreateNormalTile(_boardLayout[i].X, _boardLayout[i].Y);
            else
                _boardLayout[i].Tile.DestroyTile();
        }
        _boardLayout.Clear();
    }
}