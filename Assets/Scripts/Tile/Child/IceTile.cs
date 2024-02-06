using System.Collections.Generic;
using UnityEngine;

public class IceTile : Tile
{
    [SerializeField] List<GameObject> _ice;
    [SerializeField] List<IceBreak> _iceSprite;

    int _index;

    void Update()
    {
        if (_tileManager.IceTiles[_x, _y] == null)
            DestroyTile();
    }

    protected override void SetSprite()
    {
        for (int i = 0; i < _iceSprite.Count; i++)
            _iceSprite[i].SetSprite(_tileAtlas);
    }

    public override void Init(TileType tileType, int x, int y)
    {
        base.Init(tileType, x, y);
        _index = 0;
        for (int i = 0; i < _ice.Count; i++)
            _ice[i].SetActive(true);
    }

    public override void TakeDamage()
    {
        _ice[_index].SetActive(false);
        _index++;

        if (_index >= _ice.Count)
        {
            _tileManager.DestroyTile(this);
        }
    }

    public override void DestroyTile()
    {
        _tileManager.IceTiles[_x, _y] = null;
        _tileObjectPool.Pull(ETileKindType.Ice, gameObject);
    }
}