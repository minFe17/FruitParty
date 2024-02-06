using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

public class FruitManager : MonoBehaviour
{
    // ╫л╠шео
    Fruit[,] _allFruits;

    FactoryManager<EFruitType, Fruit> _fruitFactoryManager;
    ObjectPool<EFruitType> _fruitObjectPool;
    MatchFinder _matchFinder;
    ScoreManager _scoreManager;
    GameManager _gameManager;
    TileManager _tileManager;
    BombManager _bombManager;
    EventManager _eventManager;
    SoundManager _soundManager;
    AudioClipManager _audioClipManager;
    Fruit _currentFruit;

    Transform _parent;
    int _width;
    int _height;
    int _baseFruitScore = 20;
    int _streakValue = 1;
    float _refillDelay = 0.5f;

    public Fruit[,] AllFruits { get => _allFruits; }
    public Fruit CurrentFruit { get => _currentFruit; set => _currentFruit = value; }
    public int Width { get => _width; }
    public int Height { get => _height; }
    public int Offset { get; set; }

    public void Init(Transform parent, int x, int y)
    {
        _parent = parent;
        _width = x;
        _height = y;
        _allFruits = new Fruit[x, y];
        LoadManagers();
    }

    void LoadManagers()
    {
        _fruitFactoryManager = GenericSingleton<FactoryManager<EFruitType, Fruit>>.Instance;
        _fruitObjectPool = GenericSingleton<ObjectPool<EFruitType>>.Instance;
        _matchFinder = GenericSingleton<MatchFinder>.Instance;
        _scoreManager = GenericSingleton<ScoreManager>.Instance;
        _gameManager = GenericSingleton<GameManager>.Instance;
        _tileManager = GenericSingleton<TileManager>.Instance;
        _bombManager = GenericSingleton<BombManager>.Instance;
        _eventManager = GenericSingleton<EventManager>.Instance;
        _soundManager = GenericSingleton<SoundManager>.Instance;
        _audioClipManager = GenericSingleton<AudioClipManager>.Instance;
    }

    void DestroyMatchFruit(int column, int row)
    {
        if (_allFruits[column, row].IsMatch)
        {
            PlayMatchAudio();
            CheckTile(column, row);

            _scoreManager.AddScore(_baseFruitScore * _streakValue);
            _gameManager.AddTime(_streakValue);

            _matchFinder.MatchFruits.Remove(_allFruits[column, row]);
            DestroyFruit(_allFruits[column, row]);
            _allFruits[column, row] = null;
        }
    }

    void CheckTile(int column, int row)
    {
        CheckHaveFruitTile(column, row);
        CheckHitTile(column, row);
    }

    void CheckHaveFruitTile(int column, int row)
    {
        if (_tileManager.LockTiles[column, row] != null)
        {
            _tileManager.DestroyTile(_tileManager.LockTiles[column, row]);
            _streakValue--;
            return;
        }
        if (_tileManager.IceTiles[column, row] != null)
        {
            _tileManager.IceTiles[column, row].TakeDamage();
            _streakValue--;
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
        if (_tileManager.ConcreteTiles[column, row])
            _tileManager.DestroyTile(_tileManager.ConcreteTiles[column, row]);
    }

    void CheckLavaTile(int column, int row)
    {
        if (_tileManager.LavaTiles[column, row])
        {
            _tileManager.DestroyTile(_tileManager.LavaTiles[column, row]);
            _tileManager.CreateMoreLavaTile = false;
        }
    }

    void CheckMakeBomb()
    {
        EBombType bombType;
        Fruit fruit;
        Action<EBombType, Fruit> makeBomb = MakeableBomb(out bombType, out fruit);
        if (makeBomb != null)
            makeBomb(bombType, fruit);
    }

    Action<EBombType, Fruit> MakeableBomb(out EBombType bombType, out Fruit targetFruit)
    {
        Action<EBombType, Fruit> returnAction = null;
        List<Fruit> matchFruits = _matchFinder.MatchFruits;
        bombType = EBombType.Max;
        targetFruit = null;

        for (int i = 0; i < matchFruits.Count; i++)
        {
            List<Fruit> creatableFruits;
            Fruit fruit = matchFruits[i];
            int columnMatch;
            int rowMatch;

            Fruit returnFruit = null;
            CalculateMatch(out creatableFruits, out columnMatch, out rowMatch, matchFruits, fruit);
            if (creatableFruits.Count != 0)
            {
                EBombType checkBombType = CheckCreatableBomb(out returnFruit, creatableFruits, columnMatch, rowMatch);

                if (checkBombType != EBombType.Max)
                {
                    bombType = checkBombType;
                    targetFruit = returnFruit;
                    returnAction += _bombManager.CreateBomb;
                }
            }
        }
        return returnAction;
    }

    void CalculateMatch(out List<Fruit> creatableFruits, out int columnMatch, out int rowMatch, List<Fruit> matchFruits, Fruit fruit)
    {
        creatableFruits = new List<Fruit>();
        creatableFruits.Add(fruit);
        columnMatch = 0;
        rowMatch = 0;

        int column = fruit.Column;
        int row = fruit.Row;

        for (int i = 0; i < matchFruits.Count; i++)
        {
            Fruit nextFruit = matchFruits[i];
            if (nextFruit == fruit)
                continue;
            if (nextFruit.Column == column && nextFruit.FruitType == fruit.FruitType)
            {
                columnMatch++;
                creatableFruits.Add(nextFruit);
            }
            if (nextFruit.Row == row && nextFruit.FruitType == fruit.FruitType)
            {
                rowMatch++;
                creatableFruits.Add(nextFruit);
            }
            if (fruit.IsBomb || nextFruit.IsBomb)
            {
                if (nextFruit.Column == column && nextFruit.ColorType == fruit.ColorType)
                {
                    columnMatch++;
                    creatableFruits.Add(nextFruit);
                }
                if (nextFruit.Row == row && nextFruit.ColorType == fruit.ColorType)
                {
                    rowMatch++;
                    creatableFruits.Add(nextFruit);
                }
            }
        }
    }

    EBombType CheckCreatableBomb(out Fruit targetFruit, List<Fruit> creatableFruits, int columnMatch, int rowMatch)
    {
        targetFruit = null;

        for (int i = 0; i < creatableFruits.Count; i++)
        {
            if (creatableFruits[i] == _currentFruit)
            {
                if (columnMatch == 4 || rowMatch == 4)
                {
                    targetFruit = _currentFruit;
                    return EBombType.FruitBomb;
                }
                else if (columnMatch == 2 && rowMatch == 2)
                {
                    targetFruit = _currentFruit;
                    return EBombType.SquareBomb;
                }
                else if (columnMatch == 3 || rowMatch == 3)
                {
                    targetFruit = _currentFruit;
                    return EBombType.LineBomb;
                }
            }
            if (creatableFruits[i] == _currentFruit.OtherFruit)
            {
                if (columnMatch == 4 || rowMatch == 4)
                {
                    targetFruit = _currentFruit.OtherFruit;
                    return EBombType.FruitBomb;
                }
                else if (columnMatch == 2 && rowMatch == 2)
                {
                    targetFruit = _currentFruit.OtherFruit;
                    return EBombType.SquareBomb; ;
                }
                else if (columnMatch == 3 || rowMatch == 3)
                {
                    targetFruit = _currentFruit.OtherFruit;
                    return EBombType.LineBomb;
                }
            }
        }
        return EBombType.Max;
    }

    void RefillFruit()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_allFruits[i, j] == null && !_tileManager.BlankTiles[i, j] && _tileManager.ConcreteTiles[i, j] == null && _tileManager.LavaTiles[i, j] == null)
                {
                    int fruitNumber = Random.Range(0, _fruitFactoryManager.Count);
                    int iterations = 0;
                    while (MatchAt(i, j, (EFruitType)fruitNumber) && iterations < 100)
                    {
                        iterations++;
                        fruitNumber = Random.Range(0, _fruitFactoryManager.Count);
                    }
                    MakeFruit((EFruitType)fruitNumber, i, j);
                }
            }
        }
    }

    bool MatchOnBoard()
    {
        _matchFinder.FindAllMatch();
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_allFruits[i, j] != null)
                {
                    if (_allFruits[i, j].IsMatch)
                        return true;
                }
            }
        }
        return false;
    }

    void SwitchFruit(int column, int row, Vector2Int direction)
    {
        if (_allFruits[column + direction.x, row + direction.y] != null)
        {
            Fruit temp = _allFruits[column + direction.x, row + direction.y] as Fruit;
            _allFruits[column + (int)direction.x, row + (int)direction.y] = _allFruits[column, row];
            _allFruits[column, row] = temp;
        }
    }

    bool CheckForMatch()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_allFruits[i, j] != null)
                {
                    List<Fruit> fruitsList;
                    List<Fruit> bombs;
                    if (i < _width - 2)
                    {
                        if (_allFruits[i + 1, j] != null && _allFruits[i + 2, j] != null)
                        {
                            Fruit[] fruits = { _allFruits[i, j], _allFruits[i + 1, j], _allFruits[i + 2, j] };
                            _matchFinder.BombCount(out fruitsList, out bombs, fruits);
                            if (bombs.Count != 0)
                            {
                                if (CheckBombMatch(fruitsList, bombs))
                                    return true;
                            }

                            else if (_allFruits[i, j].FruitType == _allFruits[i + 1, j].FruitType && _allFruits[i, j].FruitType == _allFruits[i + 2, j].FruitType)
                                return true;
                        }
                    }
                    if (j < _height - 2)
                    {
                        if (_allFruits[i, j + 1] != null && _allFruits[i, j + 2] != null)
                        {
                            Fruit[] fruits = { _allFruits[i, j], _allFruits[i, j + 1], _allFruits[i, j + 2] };
                            _matchFinder.BombCount(out fruitsList, out bombs, fruits);
                            if (bombs.Count != 0)
                            {
                                if (CheckBombMatch(fruitsList, bombs))
                                    return true;
                            }

                            else if (_allFruits[i, j].FruitType == _allFruits[i, j + 1].FruitType && _allFruits[i, j].FruitType == _allFruits[i, j + 2].FruitType)
                                return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    bool CheckBombMatch(List<Fruit> fruits, List<Fruit> bombs)
    {
        if (bombs.Count == 3)
        {
            if (bombs[0].ColorType == bombs[1].ColorType && bombs[0].ColorType == bombs[2].ColorType)
                return true;
        }
        else if (bombs.Count == 2)
        {
            if (bombs[0].ColorType == bombs[1].ColorType && bombs[0].ColorType == fruits[0].ColorType)
                return true;
        }
        else if (bombs.Count == 1)
        {
            if (bombs[0].ColorType == fruits[0].ColorType && fruits[0].FruitType == fruits[1].FruitType)
                return true;
        }
        return false;
    }

    void PlayMatchAudio()
    {
        int randomAudio = Random.Range(0, _audioClipManager.FruitMatchSFX.Count);
        _soundManager.PlaySFX(_audioClipManager.FruitMatchSFX[randomAudio]);
    }

    void CheckCreateMoreLavaTile()
    {
        if (_tileManager.LavaTileInBoard() && !_tileManager.FirstCreateLavaTile)
            _tileManager.CreateMoreLavaTile = true;
        if (_tileManager.FirstCreateLavaTile)
            _tileManager.FirstCreateLavaTile = false;
    }

    public void CreateFruit(Vector2Int position)
    {
        int fruitNumber;
        int iterations = 0;
        int x = position.x;
        int y = position.y - Offset;
        do
        {
            fruitNumber = Random.Range(0, _fruitFactoryManager.Count);
            iterations++;
        }
        while (MatchAt(x, y, (EFruitType)fruitNumber) && iterations <= 100);
        MakeFruit((EFruitType)fruitNumber, x, y);
    }

    public void MakeFruit(EFruitType type, int column, int row)
    {
        Vector2Int position = new Vector2Int(column, row);
        Fruit fruit = _fruitFactoryManager.MakeObject(type, position);

        _allFruits[column, row] = fruit;
        fruit.transform.position = new Vector2(column, row + Offset);
        fruit.transform.parent = _parent;
    }

    public bool MatchAt(int column, int row, EFruitType type)
    {
        if (column > 1 && row > 1)
        {
            if (_allFruits[column - 1, row] != null && _allFruits[column - 2, row] != null)
            {
                if (_allFruits[column - 1, row].FruitType == type && _allFruits[column - 2, row].FruitType == type)
                    return true;
            }
            if (_allFruits[column, row - 1] != null && _allFruits[column, row - 2] != null)
            {
                if (_allFruits[column, row - 1].FruitType == type && _allFruits[column, row - 2].FruitType == type)
                    return true;
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (column > 1)
            {
                if (_allFruits[column - 1, row] != null && _allFruits[column - 2, row] != null)
                {
                    if (_allFruits[column - 1, row].FruitType == type && _allFruits[column - 2, row].FruitType == type)
                        return true;
                }
            }
            if (row > 1)
            {
                if (_allFruits[column, row - 1] != null && _allFruits[column, row - 2] != null)
                {
                    if (_allFruits[column, row - 1].FruitType == type && _allFruits[column, row - 2].FruitType == type)
                        return true;
                }
            }
        }
        return false;
    }

    public void BuyFruit(int column, int row)
    {
        DestroyFruit(_allFruits[column, row]);
        _allFruits[column, row] = null;
    }

    public void CheckMatchFruit()
    {
        if (_matchFinder.MatchFruits.Count >= 4)
            CheckMakeBomb();
        _matchFinder.MatchFruits.Clear();

        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_allFruits[i, j] != null)
                    DestroyMatchFruit(i, j);
            }
        }
        StartCoroutine(DecreaseRowRoutine());
    }

    public bool IsDeadlocked()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_allFruits[i, j] != null)
                {
                    if (i < _width - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2Int.right))
                            return false;
                    }
                    if (j < _height - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2Int.up))
                            return false;
                    }
                }
            }
        }
        return true;
    }

    public bool SwitchAndCheck(int column, int row, Vector2Int direction)
    {
        SwitchFruit(column, row, direction);
        if (CheckForMatch())
        {
            SwitchFruit(column, row, direction);
            return true;
        }
        else
        {
            SwitchFruit(column, row, direction);
            return false;
        }
    }

    public void DestroyFruit(Fruit fruit)
    {
        _fruitObjectPool.Pull(fruit.FruitType, fruit.gameObject);
    }

    IEnumerator DecreaseRowRoutine()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_allFruits[i, j] == null && !_tileManager.BlankTiles[i, j] && _tileManager.ConcreteTiles[i, j] == null && _tileManager.LavaTiles[i, j] == null)
                {
                    for (int k = j + 1; k < _height; k++)
                    {
                        if (_allFruits[i, k] != null)
                        {
                            _allFruits[i, k].Row = j;
                            _allFruits[i, k] = null;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(_refillDelay * 0.5f);
        StartCoroutine(FillFruitRoutine());
    }

    IEnumerator FillFruitRoutine()
    {
        yield return new WaitForSeconds(_refillDelay);
        RefillFruit();
        yield return new WaitForSeconds(_refillDelay);

        while (MatchOnBoard())
        {
            yield return new WaitForSeconds(_refillDelay);
            _streakValue++;
            CheckMatchFruit();
            yield break;
        }

        yield return new WaitForSeconds(_refillDelay);

        if (MatchOnBoard())
        {
            _streakValue++;
            CheckMatchFruit();
            yield break;
        }

        _matchFinder.MatchFruits.Clear();
        _currentFruit = null;
        _tileManager.CreateMoreLavaTiles();

        if (IsDeadlocked())
            _eventManager.Shuffle.EventEffect();
        yield return new WaitForSeconds(_refillDelay);

        System.GC.Collect();
        _gameManager.GameState = EGameStateType.Move;
        CheckCreateMoreLavaTile();
        _streakValue = 1;
    }
}

public enum EFruitType
{
    Carrot,
    Lemon,
    Orange,
    StarFruit,
    Strawberry,
    Tomato,
    Watermelon,
    Max,
}

public enum EColorType
{
    Red,
    Orange,
    Yellow,
    Green,
    Blue,
    Max,
}