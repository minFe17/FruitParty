using System.Collections.Generic;
using UnityEngine;

public class IceTile : Tile
{
    [SerializeField] List<GameObject> _ice;

    public void TakeDamage()
    {
        _ice[0].SetActive(false);
        _ice.RemoveAt(0);

        if (_ice.Count == 0)
            DestroyTile();
    }

    void DestroyTile()
    {
        _tileManager.IceTiles[_x, _y] = null;
        Destroy(this.gameObject);
    }
}