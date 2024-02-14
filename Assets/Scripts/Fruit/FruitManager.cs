using System.Collections;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

public class FruitManager : MonoBehaviour
{
    // ╫л╠шео
    Fruit[] _fruits;
    Fruit[,] _allFruits;

    FactoryManager _factoryManager;
    ObjectPoolManager _objectPoolManager;
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
    public int StreakValue { get => _streakValue; set => _streakValue = value; }

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
        _factoryManager = GenericSingleton<FactoryManager>.Instance;
        _objectPoolManager = GenericSingleton<ObjectPoolManager>.Instance;
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
            _tileManager.CheckTile(column, row);

            _scoreManager.AddScore(_baseFruitScore * _streakValue);
            _gameManager.AddTime(_streakValue);

            _matchFinder.MatchFruits.Remove(_allFruits[column, row]);
            DestroyFruit(_allFruits[column, row]);
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
                    int fruitNumber = Random.Range(0, _factoryManager.FruitCount);
                    int iterations = 0;
                    while (MatchAt(i, j, (EFruitType)fruitNumber) && iterations < 100)
                    {
                        iterations++;
                        fruitNumber = Random.Range(0, _factoryManager.FruitCount);
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
            _allFruits[column + direction.x, row + direction.y] = _allFruits[column, row];
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

                    if (i < _width - 2)
                    {
                        if (_allFruits[i + 1, j] != null && _allFruits[i + 2, j] != null)
                        {
                            _fruits = new Fruit[] { _allFruits[i, j], _allFruits[i + 1, j], _allFruits[i + 2, j] };
                            _matchFinder.CheckMatch(_fruits);
                        }
                    }
                    if (j < _height - 2)
                    {
                        if (_allFruits[i, j + 1] != null && _allFruits[i, j + 2] != null)
                        {
                            _fruits = new Fruit[] { _allFruits[i, j], _allFruits[i, j + 1], _allFruits[i, j + 2] };
                            _matchFinder.CheckMatch(_fruits);
                        }
                    }
                }
            }
        }
        return false;
    }

    void PlayMatchAudio()
    {
        int randomAudio = Random.Range(0, _audioClipManager.FruitMatchSFX.Count);
        _soundManager.PlaySFX(_audioClipManager.FruitMatchSFX[randomAudio]);
    }

    public void CreateFruit(Vector2Int position)
    {
        int fruitNumber;
        int iterations = 0;
        int x = position.x;
        int y = position.y;
        do
        {
            fruitNumber = Random.Range(0, (int)EFruitType.Max);
            iterations++;
        }
        while (MatchAt(x, y, (EFruitType)fruitNumber) && iterations <= 100);
        MakeFruit((EFruitType)fruitNumber, x, y);
    }

    public void MakeFruit(EFruitType type, int column, int row)
    {
        Vector2Int position = new Vector2Int(column, row);
        Fruit fruit = _factoryManager.MakeObject<EFruitType, Fruit>(type, position);

        _allFruits[column, row] = fruit;
        fruit.transform.position = new Vector2(column, row);
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
    }

    public void CheckMatchFruit()
    {
        if (_matchFinder.MatchFruits.Count >= 4)
            _bombManager.CheckMakeBomb();
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
        _objectPoolManager.Pull(fruit.FruitType, fruit.gameObject);
        _allFruits[fruit.Column, fruit.Row] = null;
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

        _eventManager.Shuffle.EventEffect();
        yield return new WaitForSeconds(_refillDelay);

        _gameManager.GameState = EGameStateType.Move;
        _tileManager.CheckCreateMoreLavaTile();
        _streakValue = 1;
    }
}