using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class FruitManager : MonoBehaviour
{
    // 싱글턴
    Fruit[,] _allFruits;
    List<GameObject> _fruits = new List<GameObject>();

    Board _board;
    MatchFinder _matchFinder;
    ScoreManager _scoreManager;
    GameManager _gameManager;
    SoundManager _soundManager;
    AudioClipManager _audioClipManager;
    Fruit _currentFruit;

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

    public void Init(int x, int y, Board board)
    {
        _width = x;
        _height = y;
        _board = board;
        _allFruits = new Fruit[x, y];

        _matchFinder = GenericSingleton<MatchFinder>.Instance;
        _scoreManager = GenericSingleton<ScoreManager>.Instance;
        _gameManager = GenericSingleton<GameManager>.Instance;
        _soundManager = GenericSingleton<SoundManager>.Instance;
        _audioClipManager = GenericSingleton<AudioClipManager>.Instance;
    }

    void AddFruit()
    {
        for (int i = 0; i < (int)EFruitType.Max; i++)
            _fruits.Add(Resources.Load($"Prefabs/Fruits/{(EFruitType)i}") as GameObject);
    }

    bool MatchAt(int column, int row, GameObject fruit)
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

    void DestroyMatchFruit(int column, int row)
    {
        if (_allFruits[column, row].IsMatch)
        {
            PlayMatchAudio();
            CheckIceTiles(column, row);
            
            _scoreManager.AddScore(_baseFruitScore * _streakValue);
            _gameManager.AddTime(_streakValue);
            if (_matchFinder.MatchFruits.Count >= 4)
            {
                Destroy(_allFruits[column, row].gameObject);
                CheckMakeBomb();
                if (_allFruits[column, row] != _currentFruit && _allFruits[column, row] != _currentFruit.OtherFruit)
                {
                    _matchFinder.MatchFruits.Remove(_allFruits[column, row]);
                    _allFruits[column, row] = null;
                }
            }
            else
            {
                _matchFinder.MatchFruits.Remove(_allFruits[column, row]);
                Destroy(_allFruits[column, row].gameObject);
                _allFruits[column, row] = null;
            }
        }
    }

    void CheckIceTiles(int column, int row)
    {
        if (_board.IceTiles[column, row] != null)
        {
            _board.IceTiles[column, row].TakeDamage();
            _streakValue--;
        }
    }

    void CheckMakeBomb()
    {
        EBombType makeableBomb = MakeableBombType();
        switch (makeableBomb)
        {
            case EBombType.LineBomb:
                CheckMakeLineBomb();
                break;
            case EBombType.SquareBomb:
                CheckMakeSquareBomb();
                break;
            case EBombType.FruitBomb:
                CheckMakeFruitBomb();
                break;
        }
    }

    EBombType MakeableBombType()
    {
        List<Fruit> matchFruits = _matchFinder.MatchFruits as List<Fruit>;
        for(int i=0; i< matchFruits.Count; i++)
        {
            Fruit fruit = matchFruits[i];
            int column = fruit.Column;
            int row = fruit.Row;
            int columnMatch = 0;
            int rowMatch = 0;

            for(int j = 0; j<matchFruits.Count; j++)
            {
                Fruit nextFruit = matchFruits[j];
                if (nextFruit == fruit)
                    continue;
                if(nextFruit.Column == column && nextFruit.FruitType == fruit.FruitType)
                    columnMatch++;
                if (nextFruit.Row == row && nextFruit.FruitType == fruit.FruitType)
                    rowMatch++;
                if(fruit.IsBomb || nextFruit.IsBomb)
                {
                    if (nextFruit.Column == column && nextFruit.ColorType == fruit.ColorType)
                        columnMatch++;
                    if (nextFruit.Row == row && nextFruit.ColorType == fruit.ColorType)
                        rowMatch++;
                }
            }
            if(columnMatch == 4 || rowMatch == 4)
                return EBombType.FruitBomb;
            if (columnMatch == 2 && rowMatch == 2)
                return EBombType.SquareBomb;
            if(columnMatch == 3 || rowMatch == 3)
                return EBombType.LineBomb;
        }
        return EBombType.None;
    }

    void CheckMakeLineBomb()
    {
        if (_currentFruit != null)
        {
            if (_currentFruit.IsMatch)
            {
                _currentFruit.MakeLineBomb();
            }
            else if (_currentFruit.OtherFruit != null && _currentFruit.OtherFruit.IsMatch)
            {
                _currentFruit.OtherFruit.MakeLineBomb();
            }
        }
    }

    void CheckMakeSquareBomb()
    {
        if (_currentFruit != null)
        {
            if (_currentFruit.IsMatch)
            {
                _currentFruit.MakeSquareBomb();
            }
            else if (_currentFruit.OtherFruit != null && _currentFruit.OtherFruit.IsMatch)
            {
                _currentFruit.OtherFruit.MakeSquareBomb();
            }
        }
    }

    void CheckMakeFruitBomb()
    {
        if (_currentFruit != null)
        {
            if (_currentFruit.IsMatch)
            {
                if (!_currentFruit.IsBomb)
                    _currentFruit.MakeFruitBomb();
                else if (_currentFruit.BombType != EBombType.FruitBomb)
                    _currentFruit.MakeFruitBomb();
            }
            else if (_currentFruit.OtherFruit != null && _currentFruit.OtherFruit.IsMatch)
            {
                if (!_currentFruit.OtherFruit.IsBomb)
                    _currentFruit.MakeFruitBomb();
                else if (_currentFruit.OtherFruit.BombType != EBombType.FruitBomb)
                    _currentFruit.MakeFruitBomb();
            }
        }
    }

    void RefillFruit()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_allFruits[i, j] == null && !_board.BlankSpaces[i, j])
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

    bool IsDeadlocked()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_allFruits[i, j] != null)
                {
                    if (i < _width - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.right))
                            return false;
                    }
                    if (j < _height - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                            return false;
                    }
                }
            }
        }
        return true;
    }

    void SwitchFruit(int column, int row, Vector2 direction)
    {
        Fruit temp = _allFruits[column + (int)direction.x, row + (int)direction.y] as Fruit;
        _allFruits[column + (int)direction.x, row + (int)direction.y] = _allFruits[column, row];
        _allFruits[column, row] = temp;
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

        GameObject fruit = Instantiate(_fruits[fruitNumber], position, Quaternion.identity);
        fruit.GetComponent<Fruit>().Column = x;
        fruit.GetComponent<Fruit>().Row = y;
        fruit.transform.parent = parent;
        _allFruits[x, y] = fruit.GetComponent<Fruit>();
    }

    public void CheckMatchFruit()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_allFruits[i, j] != null)
                    DestroyMatchFruit(i, j);
            }
        }
        _matchFinder.MatchFruits.Clear();
        StartCoroutine(DecreaseRowRoutine());
    }

    public bool SwitchAndCheck(int column, int row, Vector2 direction)
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
                if (!_board.BlankSpaces[i, j] && _allFruits[i, j] == null)
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

        while (MatchOnboard())
        {
            _streakValue++;
            yield return new WaitForSeconds(_refillDelay * 2);
            CheckMatchFruit();
        }
        _matchFinder.MatchFruits.Clear();
        _currentFruit = null;
        yield return new WaitForSeconds(_refillDelay);

        if (IsDeadlocked())
            StartCoroutine(ShuffleFruit());
        _gameManager.GameState = EGameStateType.Move;
        _streakValue = 1;
    }

    IEnumerator ShuffleFruit()
    {
        //이미지 보이기
        yield return new WaitForSeconds(0.5f / 2);
        _gameManager.GameState = EGameStateType.Pause;

        List<Fruit> newFruit = new List<Fruit>();
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_allFruits[i, j] != null)
                    newFruit.Add(_allFruits[i, j]);
            }
        }

        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_board.BlankSpaces[i, j] == false)
                {
                    int fruitIndex = Random.Range(0, newFruit.Count);

                    int iterations = 0;
                    while (MatchAt(i, j, newFruit[fruitIndex].gameObject) && iterations <= 100)
                    {
                        fruitIndex = Random.Range(0, _fruits.Count);
                        iterations++;
                    }

                    Fruit fruit = newFruit[fruitIndex];
                    fruit.Column = i;
                    fruit.Row = j;
                    _allFruits[i, j] = newFruit[fruitIndex];
                    newFruit.Remove(newFruit[fruitIndex]);
                }
            }
        }
        if (IsDeadlocked())
            ShuffleFruit();
        else
        {
            //이미지 숨기기
            yield return new WaitForSeconds(0.5f / 2);
            _gameManager.GameState = EGameStateType.Move;
        }
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