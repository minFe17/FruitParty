using System.Collections.Generic;
using UnityEngine;

public class IceTile : MonoBehaviour
{
    [SerializeField] List<GameObject> _iceBreakIndex;

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
        _iceBreakIndex[0].SetActive(false);
        _iceBreakIndex.RemoveAt(0);

        if (_iceBreakIndex.Count == 0)
        {
            _board.IceTiles[_x, _y] = null;
            Destroy(this.gameObject);
        }
    }
}