using System.Collections.Generic;
using UnityEngine;

public class IceTile : MonoBehaviour
{
    [SerializeField] List<GameObject> _ice;

    Board _board;
    int _x;
    int _y;

    public void Init(Board board, int x, int y)
    {
        _board = board;
        _x = x;
        _y = y;
    }

    public void TakeDamage()
    {
        _ice[0].SetActive(false);
        _ice.RemoveAt(0);

        if (_ice.Count == 0)
        {
            _board.IceTiles[_x, _y] = null;
            Destroy(this.gameObject);
        }
    }
}