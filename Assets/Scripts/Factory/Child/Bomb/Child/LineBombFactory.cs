using UnityEngine;

public class LineBombFactory : BombFactoryBase, IFactory<Bomb>
{
    protected override void Init()
    {
        _bombType = EBombType.LineBomb;
        _factoryManager.AddFactorys(_bombType, this);
    }

    public Bomb MakeObject(Vector2Int position)
    {
        GameObject temp = _objectPoolManager.Push(_bombType, _prefab);
        Bomb bomb = temp.GetComponent<Bomb>();
        bomb.Column = position.x;
        bomb.Row = position.y;
        return bomb;
    }
}