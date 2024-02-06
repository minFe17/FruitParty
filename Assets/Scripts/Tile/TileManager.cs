using System.Collections.Generic;
using UnityEngine;
using Utils;

public class TileManager : MonoBehaviour
{
    // ╫л╠шео
    public List<TileType> _boardLayout = new List<TileType>();
    GameObject _tileParent;
    FruitManager _fruitManager;
    FactoryManager<ETileKindType, Tile> _tileFactoryManager;

    [Header("Tile Array")]
    GameObject[,] _allTiles;
    bool[,] _blankTiles;
    Tile[,] _iceTiles;
    Tile[,] _lockTiles;
    Tile[,] _concreteTiles;
    Tile[,] _lavaTiles;

    int _width;
    int _height;
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
        _tileFactoryManager = GenericSingleton<FactoryManager<ETileKindType, Tile>>.Instance;
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

    void CheckFruit(int column, int row)
    {
        if (_fruitManager.AllFruits[column, row] != null)
        {
            _fruitManager.DestroyFruit(_fruitManager.AllFruits[column, row]);
            _fruitManager.AllFruits[column, row] = null;
        }
    }

    Tile CreateTiles(ETileKindType tileType, int column, int row)
    {
        Vector2Int position = new Vector2Int(column, row);
        Tile tile = _tileFactoryManager.MakeObject(tileType, position);
        TileType layout = AddBoardLayout(tileType, tile, column, row);
        tile.Init(layout, column, row);
        return tile;
    }

    void CalculateHaveFruitTile(out int column, out int row, ETileKindType type)
    {
        int tileX = 0;
        int tileY = 0;
        column = 0;
        row = 0;
        bool canCreateTilePosition = true;
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

    void CalculateObstacleTile(out int column, out int row)
    {
        int tileX = 0;
        int tileY = 0;
        column = 0;
        row = 0;
        bool canCreateTilePosition = true;
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
                {
                    canCreateTilePosition = true;
                }
            }
        }
        while (!canCreateTilePosition);
    }

    TileType AddBoardLayout(ETileKindType type, Tile tile, int column, int row)
    {
        TileType tileType = new TileType(type, tile, column, row);
        _boardLayout.Add(tileType);
        return tileType;
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

    public void CreateNormalTile(int column, int row)
    {
        Vector2Int position = new Vector2Int(column, row);
        Tile tile = _tileFactoryManager.MakeObject(ETileKindType.Normal, position);
        tile.transform.parent = _tileParent.transform;
        _allTiles[column, row] = tile.gameObject;
    }

    public void CreateBlankTiles()
    {
        int column;
        int row;
        CalculateObstacleTile(out column, out row);
        AddBoardLayout(ETileKindType.Blank, null, column, row);
        _blankTiles[column, row] = true;
        if (_allTiles[column, row] != null)
        {
            _allTiles[column, row].GetComponent<Tile>().DestroyTile();
            _fruitManager.DestroyFruit(_fruitManager.AllFruits[column, row]);
            CheckFruit(column, row);
        }
    }

    public void CreateIceTiles()
    {
        int column;
        int row;
        CalculateHaveFruitTile(out column, out row, ETileKindType.Ice);
        Tile tile = CreateTiles(ETileKindType.Ice, column, row);
        _iceTiles[column, row] = tile;
    }

    public void CreateLockTiles()
    {
        int column;
        int row;
        CalculateHaveFruitTile(out column, out row, ETileKindType.Lock);
        Tile tile = CreateTiles(ETileKindType.Lock, column, row);
        _lockTiles[column, row] = tile;
    }

    public void CreateConcreteTiles()
    {
        int column;
        int row;
        CalculateObstacleTile(out column, out row);
        CheckFruit(column, row);
        Tile tile = CreateTiles(ETileKindType.Concrete, column, row);
        _concreteTiles[column, row] = tile;
    }

    public void CreateLavaTiles()
    {
        int column;
        int row;
        CalculateObstacleTile(out column, out row);
        CheckFruit(column, row);
        Tile tile = CreateTiles(ETileKindType.Lava, column, row);
        _lavaTiles[column, row] = tile;
    }

    public bool LavaTileInBoard()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_lavaTiles[i, j] != null)
                {
                    return true;
                }
            }
        }
        return false;
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
                        int newColumn = column + direction.x;
                        int newRow = row + direction.y;

                        CheckFruit(newColumn, newRow);
                        Tile lavaTile = CreateTiles(ETileKindType.Lava, newColumn, newRow);
                        _lavaTiles[newColumn, newRow] = lavaTile;
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