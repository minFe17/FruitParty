using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] int _width;
    [SerializeField] int _height;
    Tile[,] _allTiles;
    GameObject _tilePrefab;

    void Start()
    {
        _allTiles = new Tile[_width, _height];
        _tilePrefab = Resources.Load("Prefabs/Tile") as GameObject;
        Init();
    }

    void Init()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                Vector2 position = new Vector2(i, j);
                GameObject tile = Instantiate(_tilePrefab, position, Quaternion.identity);
                tile.transform.parent = this.transform;
                tile.name = $"Tile ({i}, {j})";
            }
        }
    }
}
