using System.Collections.Generic;
using UnityEngine;
using Utils;

public class BombManager : MonoBehaviour
{
    // ╫л╠шео
    List<Fruit> _creatableFruits = new List<Fruit>();

    FactoryManager _factoryManager;
    FruitManager _fruitManager;
    TileManager _tileManager;
    MatchFinder _matchFinder;
    ELineBombDirectionType _lineBombDirection;

    public ELineBombDirectionType LineBombDirection { get => _lineBombDirection; set => _lineBombDirection = value; }

    public void Init()
    {
        _factoryManager = GenericSingleton<FactoryManager>.Instance;
        _fruitManager = GenericSingleton<FruitManager>.Instance;
        _tileManager = GenericSingleton<TileManager>.Instance;
        _matchFinder = GenericSingleton<MatchFinder>.Instance;
    }

    void CheckCreatableBomb(Fruit fruit)
    {
        int columnMatch;
        int rowMatch;
        CalculateMatch(out columnMatch, out rowMatch, fruit);

        if (_creatableFruits.Count != 0)
        {
            Fruit currentFruit = _fruitManager.CurrentFruit;

            for (int i = 0; i < _creatableFruits.Count; i++)
            {
                if (_creatableFruits[i] == currentFruit)
                    SelectCreateBomb(columnMatch, rowMatch, currentFruit);
                if (_creatableFruits[i] == currentFruit.OtherFruit)
                    SelectCreateBomb(columnMatch, rowMatch, currentFruit.OtherFruit);
            }
        }
        _creatableFruits.Clear();
    }

    void CalculateMatch(out int columnMatch, out int rowMatch, Fruit fruit)
    {
        List<Fruit> matchFruits = _matchFinder.MatchFruits;
        _creatableFruits.Add(fruit);
        columnMatch = 0;
        rowMatch = 0;

        for (int i = 0; i < matchFruits.Count; i++)
        {
            Fruit nextFruit = matchFruits[i];
            if (nextFruit == fruit)
                continue;
            if (CalulateColumnMatch(fruit, nextFruit))
            {
                columnMatch++;
                _creatableFruits.Add(nextFruit);
            }

            if (CalulateRowMatch(fruit, nextFruit))
            {
                rowMatch++;
                _creatableFruits.Add(nextFruit);
            }
        }
    }

    bool CalulateColumnMatch(Fruit fruit, Fruit nextFruit)
    {
        if (fruit.IsBomb || nextFruit.IsBomb)
        {
            if (fruit.Column == nextFruit.Column && fruit.ColorType == nextFruit.ColorType)
                return true;
        }
        if (fruit.Column == nextFruit.Column && fruit.FruitType == nextFruit.FruitType)
            return true;
        return false;
    }

    bool CalulateRowMatch(Fruit fruit, Fruit nextFruit)
    {
        if (fruit.IsBomb || nextFruit.IsBomb)
        {
            if (fruit.Row == nextFruit.Row && fruit.ColorType == nextFruit.ColorType)
                return true;
        }
        if (fruit.Row == nextFruit.Row && fruit.FruitType == nextFruit.FruitType)
            return true;
        return false;
    }

    void SelectCreateBomb(int columnMatch, int rowMatch, Fruit fruit)
    {
        if (columnMatch == 4 || rowMatch == 4)
        {
            CreateBomb(EBombType.FruitBomb, fruit);
        }
        else if (columnMatch == 2 && rowMatch == 2)
        {
            CreateBomb(EBombType.SquareBomb, fruit);
        }
        else if (columnMatch == 3 || rowMatch == 3)
        {
            CreateBomb(EBombType.LineBomb, fruit);
        }
    }

    void CreateBomb(EBombType bombType, Fruit fruit)
    {
        Vector2Int position = new Vector2Int(fruit.Column, fruit.Row);
        _factoryManager.ColorType = fruit.ColorType;
        Bomb bomb = _factoryManager.MakeObject<EBombType, Bomb>(bombType, position);
        bomb.transform.position = fruit.transform.position;

        _fruitManager.DestroyFruit(fruit);
        _fruitManager.AllFruits[fruit.Column, fruit.Row] = bomb;
        _matchFinder.MatchFruits.Clear();
    }

    void CheckConcreteTile(int column, int row)
    {
        if (_tileManager.ConcreteTiles[column, row])
        {
            _tileManager.DestroyTile(_tileManager.ConcreteTiles[column, row]);
        }
    }

    void CheckLavaTile(int column, int row)
    {
        if (_tileManager.LavaTiles[column, row])
        {
            _tileManager.DestroyTile(_tileManager.LavaTiles[column, row]);
            _tileManager.CreateMoreLavaTile = false;
        }
    }

    public void CheckMakeBomb()
    {
        List<Fruit> matchFruits = _matchFinder.MatchFruits;

        for (int i = 0; i < matchFruits.Count; i++)
        {
            CheckCreatableBomb(matchFruits[i]);
        }
    }

    public void CheckTile(int column, int row)
    {
        CheckConcreteTile(column, row);
        CheckLavaTile(column, row);
    }

    public void ChangeLineDirection()
    {
        if (_lineBombDirection == ELineBombDirectionType.Column)
            _lineBombDirection = ELineBombDirectionType.Row;
        else if (_lineBombDirection == ELineBombDirectionType.Row)
            _lineBombDirection = ELineBombDirectionType.Column;
    }
}