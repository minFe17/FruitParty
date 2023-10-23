using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

public class FruitManager : MonoBehaviour
{
    // ╫л╠шео
    List<GameObject> _fruits = new List<GameObject>();
    Fruit[,] _allFruits;

    MatchFinder _matchFinder;
    ScoreManager _scoreManager;
    GameManager _gameManager;
    TileManager _tileManager;
    EventManager _eventManager;
    SoundManager _soundManager;
    AudioClipManager _audioClipManager;
    Fruit _currentFruit;

    int _width;
    int _height;
    int _baseFruitScore = 20;
    int _streakValue = 1;
    float _refillDelay = 0.5f;

    public List<GameObject> Fruits { get => _fruits; }
    public Fruit[,] AllFruits { get => _allFruits; }
    public Fruit CurrentFruit { get => _currentFruit; set => _currentFruit = value; }
    public int Width { get => _width; }
    public int Height { get => _height; }
    public int Offset { get; set; }

    public void Init(int x, int y)
    {
        _width = x;
        _height = y;
        _allFruits = new Fruit[x, y];

        _matchFinder = GenericSingleton<MatchFinder>.Instance;
        _scoreManager = GenericSingleton<ScoreManager>.Instance;
        _gameManager = GenericSingleton<GameManager>.Instance;
        _tileManager = GenericSingleton<TileManager>.Instance;
        _eventManager = GenericSingleton<EventManager>.Instance;
        _soundManager = GenericSingleton<SoundManager>.Instance;
        _audioClipManager = GenericSingleton<AudioClipManager>.Instance;
    }

    void AddFruit()
    {
        for (int i = 0; i < (int)EFruitType.Max; i++)
            _fruits.Add(Resources.Load($"Prefabs/Fruits/{(EFruitType)i}") as GameObject);
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
            Destroy(_allFruits[column, row].gameObject);
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

    void CheckMakeBomb()
    {
        EBombType makeableBomb = MakeableBombType();
        Action makeBomb = null;

        if (makeableBomb == EBombType.LineBomb)
            makeBomb += _currentFruit.MakeLineBomb;
        else if (makeableBomb == EBombType.SquareBomb)
            makeBomb += _currentFruit.MakeSquareBomb;
        else if (makeableBomb == EBombType.FruitBomb)
            makeBomb += _currentFruit.MakeFruitBomb;

        if (makeBomb != null)
            MakeBomb(makeBomb);
    }

    EBombType MakeableBombType()
    {
        List<Fruit> matchFruits = _matchFinder.MatchFruits as List<Fruit>;
        for (int i = 0; i < matchFruits.Count; i++)
        {
            Fruit fruit = matchFruits[i];
            int column = fruit.Column;
            int row = fruit.Row;
            int columnMatch = 0;
            int rowMatch = 0;

            for (int j = 0; j < matchFruits.Count; j++)
            {
                Fruit nextFruit = matchFruits[j];
                if (nextFruit == fruit)
                    continue;
                if (nextFruit.Column == column && nextFruit.FruitType == fruit.FruitType)
                    columnMatch++;
                if (nextFruit.Row == row && nextFruit.FruitType == fruit.FruitType)
                    rowMatch++;
                if (fruit.IsBomb || nextFruit.IsBomb)
                {
                    if (nextFruit.Column == column && nextFruit.ColorType == fruit.ColorType)
                        columnMatch++;
                    if (nextFruit.Row == row && nextFruit.ColorType == fruit.ColorType)
                        rowMatch++;
                }
            }
            if (columnMatch == 4 || rowMatch == 4)
                return EBombType.FruitBomb;
            else if (columnMatch == 2 && rowMatch == 2)
                return EBombType.SquareBomb;
            else if (columnMatch == 3 || rowMatch == 3)
                return EBombType.LineBomb;
        }
        return EBombType.None;
    }

    void MakeBomb(Action makeBomb)
    {
        if (_currentFruit != null)
        {
            if (_currentFruit.IsMatch)
            {
                makeBomb();
            }
            else if (_currentFruit.OtherFruit != null && _currentFruit.OtherFruit.IsMatch)
            {
                makeBomb();
            }
        }
    }

    void RefillFruit()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_allFruits[i, j] == null && !_tileManager.BlankTiles[i, j] && _tileManager.ConcreteTiles[i, j] == null && _tileManager.LavaTiles[i, j] == null)
                {
                    Vector2 position = new Vector2(i, j + Offset);
                    int fruitNumber = Random.Range(0, _fruits.Count);
                    int iterations = 0;

                    while (MatchAt(i, j, _fruits[fruitNumber]) && iterations < 100)
                    {
                        iterations++;
                        fruitNumber = Random.Range(0, _fruits.Count);
                    }

                    iterations = 0;
                    GameObject fruit = Instantiate(_fruits[fruitNumber], position, Quaternion.identity);
                    _allFruits[i, j] = fruit.GetComponent<Fruit>();
                    fruit.GetComponent<Fruit>().Column = i;
                    fruit.GetComponent<Fruit>().Row = j;
                }
            }
        }
    }

    bool MatchOnboard()
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

    public void CreateFruit(Transform parent, Vector2 position)
    {
        if (_fruits.Count == 0)
            AddFruit();
        int fruitNumber = 0;
        int iterations = 0;
        int x = (int)position.x;
        int y = (int)position.y - Offset;
        do
        {
            fruitNumber = Random.Range(0, _fruits.Count);
            iterations++;
        }
        while (MatchAt(x, y, _fruits[fruitNumber]) && iterations <= 100);

        GameObject temp = Instantiate(_fruits[fruitNumber], position, Quaternion.identity);
        Fruit fruit = temp.GetComponent<Fruit>();
        fruit.Column = x;
        fruit.Row = y;
        temp.transform.parent = parent;
        _allFruits[x, y] = fruit;
    }

    public bool MatchAt(int column, int row, GameObject fruit)
    {
        Fruit fruitType = fruit.GetComponent<Fruit>();

        if (column > 1 && row > 1)
        {
            if (_allFruits[column - 1, row] != null && _allFruits[column - 2, row] != null)
            {
                if (_allFruits[column - 1, row].FruitType == fruitType.FruitType && _allFruits[column - 2, row].FruitType == fruitType.FruitType)
                    return true;
            }
            if (_allFruits[column, row - 1] != null && _allFruits[column, row - 2] != null)
            {
                if (_allFruits[column, row - 1].FruitType == fruitType.FruitType && _allFruits[column, row - 2].FruitType == fruitType.FruitType)
                    return true;
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (column > 1)
            {
                if (_allFruits[column - 1, row] != null && _allFruits[column - 2, row] != null)
                {
                    if (_allFruits[column - 1, row].FruitType == fruitType.FruitType && _allFruits[column - 2, row].FruitType == fruitType.FruitType)
                        return true;
                }
            }
            if (row > 1)
            {
                if (_allFruits[column, row - 1] != null && _allFruits[column, row - 2] != null)
                {
                    if (_allFruits[column, row - 1].FruitType == fruitType.FruitType && _allFruits[column, row - 2].FruitType == fruitType.FruitType)
                        return true;
                }
            }
        }
        return false;
    }

    public void BuyFruit(int column, int row)
    {
        Destroy(_allFruits[column, row].gameObject);
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

        while (MatchOnboard())
        {
            yield return new WaitForSeconds(_refillDelay);
            _streakValue++;
            CheckMatchFruit();
            yield break;
        }

        yield return new WaitForSeconds(_refillDelay);

        if(MatchOnboard())
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