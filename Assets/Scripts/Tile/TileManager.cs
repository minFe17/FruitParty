using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    // ╫л╠шео
    List<TileType> _boardLayout = new List<TileType>();
    GameObject _tiles;

    [Header("Tile Array")]
    GameObject[,] _allTiles;
    bool[,] _blankTiles;
    IceTile[,] _iceTiles;
    LockTile[,] _lockTiles;
    ConcreteTile[,] _concreteTiles;

    [Header("Tile Prefab")]
    GameObject _tilePrefab;
    GameObject _iceTilePrefab;
    GameObject _lockTilePrefab;
    GameObject _concreteTilePrefab;

    public bool[,] BlankTiles { get => _blankTiles; }
    public IceTile[,] IceTiles { get => _iceTiles; }
    public LockTile[,] LockTiles { get => _lockTiles; }
    public ConcreteTile[,] ConcreteTiles { get => _concreteTiles; }

    public void Init(int width, int height)
    {
        SetArray(width, height);
        LoadTilePrefab();
        _tiles = new GameObject("Tiles");
    }

    void SetArray(int width, int height)
    {
        _allTiles = new GameObject[width, height];
        _blankTiles = new bool[width, height];
        _iceTiles = new IceTile[width, height];
        _lockTiles = new LockTile[width, height];
        _concreteTiles = new ConcreteTile[width, height];
    }

    void LoadTilePrefab()
    {
        _tilePrefab = Resources.Load("Prefabs/Tile/Tile") as GameObject;
        _iceTilePrefab = Resources.Load("Prefabs/Tile/IceTile") as GameObject;
        _lockTilePrefab = Resources.Load("Prefabs/Tile/LockTile") as GameObject;
        _concreteTilePrefab = Resources.Load("Prefabs/Tile/LockTile") as GameObject;
    }

    void GenerateTiles()
    {
        for (int i = 0; i < _boardLayout.Count; i++)
        {
            switch(_boardLayout[i].TileKindType)
            {
                case ETileKindType.Blank:
                    GenerateBlankTiles(_boardLayout[i]);
                    break;
                case ETileKindType.Ice:
                    GenerateIceTiles(_boardLayout[i]);
                    break;
                case ETileKindType.Lock:
                    GenerateLockTiles(_boardLayout[i]);
                    break;
                case ETileKindType.Concrete:
                    GenerateConcreteTiles(_boardLayout[i]);
                    break;
            }
        }
    }

    void GenerateBlankTiles(TileType tile)
    {
        int x = tile.X;
        int y = tile.Y;
        _blankTiles[x, y] = true;
        if (_allTiles[x, y] != null)
        {
            Destroy(_allTiles[x, y]);
            _allTiles[x, y] = null;
        }
    }

    void GenerateIceTiles(TileType tile)
    {
        Vector2 position = new Vector3(tile.X, tile.Y);
        GameObject iceTile = Instantiate(_iceTilePrefab, position, Quaternion.identity);
        _iceTiles[tile.X, tile.Y] = iceTile.GetComponent<IceTile>();
        iceTile.GetComponent<IceTile>().Init(tile.X, tile.Y);
    }

    void GenerateLockTiles(TileType tile)
    {
        Vector2 position = new Vector3(tile.X, tile.Y);
        GameObject lockTile = Instantiate(_lockTilePrefab, position, Quaternion.identity);
        _lockTiles[tile.X, tile.Y] = lockTile.GetComponent<LockTile>();
        lockTile.GetComponent<LockTile>().Init(tile.X, tile.Y);
    }

    void GenerateConcreteTiles(TileType tile)
    {
        Vector2 position = new Vector3(tile.X, tile.Y);
        GameObject concreteTile = Instantiate(_concreteTilePrefab, position, Quaternion.identity);
        _concreteTiles[tile.X, tile.Y] = concreteTile.GetComponent<ConcreteTile>();
        concreteTile.GetComponent<ConcreteTile>().Init(tile.X, tile.Y);
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
}