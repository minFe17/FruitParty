using System.Collections;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

public class FruitManager : MonoBehaviour
{
    // �̱���
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
        // ���� �Ŵ��� �ʱ�ȭ: �θ� Transform ���� �� ���� ũ�� ����, �Ŵ��� �ν��Ͻ� �ε�
        _parent = parent;
        _width = x;
        _height = y;
        _allFruits = new Fruit[x, y];
        LoadManagers();
    }

    void LoadManagers()
    {
        // �̱��� �Ŵ����� �ν��Ͻ� ��������
        _factoryManager = GenericSingleton<FactoryManager>.Instance;
        _objectPoolManager = GenericSingleton<ObjectPoolManager>.Instance;
        _matchFinder = GenericSingleton<MatchFinder>.Instance;
        _scoreManager = GenericSingleton<ScoreManager>.Instance;
        _gameManager = GenericSingleton<GameManager>.Instance;
        _tileManager = GenericSingleton<TileManager>.Instance;
        _bombManager = GenericSingleton<BombManager>.Instance;
        _eventManager = GenericSingleton<EventManager>.Instance;
        _audioClipManager = GenericSingleton<AudioClipManager>.Instance;
    }

    /// <summary>
    /// ��ġ�� ������ ó��: ������ �ð� ����, Ÿ�� ���� üũ, ��ġ ����Ʈ���� ���� �� ���� ����
    /// </summary>
    void DestroyMatchFruit(int column, int row)
    {
        Fruit fruit = _allFruits[column, row];

        if (fruit != null && fruit.IsMatch)
        {
            PlayMatchAudio();

            // �ش� ��ġ Ÿ�� ���� üũ
            _tileManager.CheckTile(column, row);

            _scoreManager.AddScore(_baseFruitScore * _streakValue); 
            _gameManager.AddTime(_streakValue);

            // ��ġ ����Ʈ���� ���� ����
            _matchFinder.MatchFruits.Remove(fruit); 
            DestroyFruit(fruit);                    
        }
    }

    void RefillFruit()
    {
        // �� ������ �� ���� ���� (Ÿ�� ���� Ȯ�� ��)
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_allFruits[i, j] == null && !_tileManager.BlankTiles[i, j] && _tileManager.ConcreteTiles[i, j] == null && _tileManager.LavaTiles[i, j] == null)
                {
                    SelectCreateFruit(i, j);
                }
            }
        }
    }

    /// <summary>
    /// ���� ��ġ�� ��ġ���� �ʴ� ���� Ÿ���� �������� �����Ͽ� ����
    /// </summary>
    void SelectCreateFruit(int x, int y)
    {
        // ���� Ÿ�� ���� ����, ���� ��ġ ���� ���� �ݺ� üũ
        int fruitNumber;
        int iterations = 0;
        do
        {
            fruitNumber = Random.Range(0, (int)EFruitType.Max);
            iterations++;
        }
        while (MatchAt(x, y, (EFruitType)fruitNumber) && iterations <= 100);
        MakeFruit((EFruitType)fruitNumber, x, y);
    }

    /// <summary>
    /// ���� ���忡 ��ġ�� ������ �ִ��� �˻��ϰ�, ������ true�� ��ȯ
    /// </summary>
    bool MatchOnBoard()
    {
        // ���� ��ü�� �˻��� ��ġ�� ������ �ִ��� Ȯ��
        _matchFinder.FindAllMatch();  // ��� ��ġ���� ��ġ ���� Ž�� ����
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_allFruits[i, j] != null && _allFruits[i, j].IsMatch)
                    return true;  // ��ġ ���� �߰� �� ��� true ��ȯ
            }
        }
        return false;  // ��ġ ���� ������ false ��ȯ
    }


    bool CheckColumnMatch(int column, int row, EFruitType type)
    {
        // ���� �������� 3��ġ�� �ִ��� �˻�
        if (_allFruits[column - 1, row] != null && _allFruits[column - 2, row] != null)
        {
            if (_allFruits[column - 1, row].FruitType == type && _allFruits[column - 2, row].FruitType == type)
                return true;
        }
        return false;
    }

    bool CheckRowMatch(int column, int row, EFruitType type)
    {
        // ���� �������� 3��ġ�� �ִ��� �˻�
        if (_allFruits[column, row - 1] != null && _allFruits[column, row - 2] != null)
        {
            if (_allFruits[column, row - 1].FruitType == type && _allFruits[column, row - 2].FruitType == type)
                return true;
        }
        return false;
    }

    void SwitchFruit(int column, int row, Vector2Int direction)
    {
        // ������ �� ���� ��ġ�� ��ȯ
        if (_allFruits[column + direction.x, row + direction.y] != null)
        {
            Fruit temp = _allFruits[column + direction.x, row + direction.y];
            _allFruits[column + direction.x, row + direction.y] = _allFruits[column, row];
            _allFruits[column, row] = temp;
        }
    }

    /// <summary>
    /// Ư�� ��ġ���� ���� 3��ġ�� �ִ��� �˻�
    /// </summary>
    bool CheckHorizontalMatch(int x, int y)
    {
        if (x + 2 >= _width) return false;
        if (_allFruits[x + 1, y] == null || _allFruits[x + 2, y] == null) return false;

        Fruit[] fruits = { _allFruits[x, y], _allFruits[x + 1, y], _allFruits[x + 2, y] };
        return _matchFinder.CheckMatch(fruits);
    }

    /// <summary>
    /// Ư�� ��ġ���� ���� 3��ġ�� �ִ��� �˻�
    /// </summary>
    bool CheckVerticalMatch(int x, int y)
    {
        if (y + 2 >= _height) return false;
        if (_allFruits[x, y + 1] == null || _allFruits[x, y + 2] == null) return false;

        Fruit[] fruits = { _allFruits[x, y], _allFruits[x, y + 1], _allFruits[x, y + 2] };
        return _matchFinder.CheckMatch(fruits);
    }

    /// <summary>
    /// ��ü ���忡 3��ġ �̻��� �����ϴ��� �˻�
    /// </summary>
    bool CheckForMatch()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (_allFruits[x, y] == null) continue;

                if (CheckHorizontalMatch(x, y) || CheckVerticalMatch(x, y))
                    return true;
            }
        }
        return false;
    }


    void PlayMatchAudio()
    {
        // ��ġ ȿ���� ���
        _audioClipManager.PlayMatchSFX();
    }

    public void CreateFruit(Vector2Int position)
    {
        // ������ ��ġ�� ���� ���� �õ�
        int x = position.x;
        int y = position.y;
        SelectCreateFruit(x, y);
    }

    public void MakeFruit(EFruitType type, int column, int row)
    {
        // ���� ��ü ���� �� �ʱ� ��ġ ����
        Vector2Int position = new Vector2Int(column, row);
        Fruit fruit = _factoryManager.MakeObject<EFruitType, Fruit>(type, position);

        _allFruits[column, row] = fruit;
        fruit.transform.position = new Vector2(column, row);
        fruit.transform.parent = _parent;
    }

    public bool MatchAt(int column, int row, EFruitType type)
    {
        // Ư�� ��ġ���� ���� ���� �� 3��ġ �߻� ���� �˻�
        if (column > 1 && row > 1)
        {
            if (CheckColumnMatch(column, row, type))
                return true;
            if (CheckRowMatch(column, row, type))
                return true;
        }
        else if (column <= 1 || row <= 1)
        {
            if (column > 1)
            {
                if (CheckColumnMatch(column, row, type))
                    return true;
            }
            if (row > 1)
            {
                if (CheckRowMatch(column, row, type))
                    return true;
            }
        }
        return false;
    }

    public void BuyFruit(int column, int row)
    {
        // �ܺ� ��û�� ���� ���� ���� (��: ���� ��)
        DestroyFruit(_allFruits[column, row]);
    }

    /// <summary>
    /// ��ġ�� ���� ó��: ��ź ���� ���� Ȯ�� �� ��ġ ���� ���� �� �� �������� ������ �ڷ�ƾ ����
    /// </summary>
    public void CheckMatchFruit()
    {
        if (_matchFinder.MatchFruits.Count >= 4)
            _bombManager.CheckMakeBomb();

        RemoveMatchedFruits();
        StartCoroutine(DecreaseRowRoutine());
    }

    /// <summary>
    /// ��ġ�� ������ �����ϰ� ��ġ ����Ʈ�� �ʱ�ȭ
    /// </summary>
    void RemoveMatchedFruits()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_allFruits[i, j] != null && _allFruits[i, j].IsMatch)
                    DestroyMatchFruit(i, j);
            }
        }
        _matchFinder.MatchFruits.Clear();
    }

    /// <summary>
    /// ������ �������� ���� ��ġ�� ��ȯ�� ��, ��ġ�� �߻��ϴ��� �˻�
    /// �˻� �� ���� ��ġ�� ����
    /// </summary>
    public bool SwitchAndCheck(int column, int row, Vector2Int direction)
    {
        // ��ġ ��ȯ
        SwitchFruit(column, row, direction);

        // ��ġ �˻�
        bool isMatch = CheckForMatch();

        // ��ġ ����
        SwitchFruit(column, row, direction);
        return isMatch;
    }

    public void DestroyFruit(Fruit fruit)
    {
        // ���� ������Ʈ Ǯ�� ��ȯ �� ���� �迭���� ����
        _objectPoolManager.Push(fruit.FruitType, fruit.gameObject);
        _allFruits[fruit.Column, fruit.Row] = null;
    }

    public void DestroyAllFruit()
    {
        // ��ü ���� ����
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_allFruits[i, j] != null)
                    DestroyFruit(_allFruits[i, j]);
            }
        }
    }

    /// <summary>
    /// ���忡�� �� ������ ã�� ���� �ִ� ������ �Ʒ��� �̵�
    /// �̵� �� ���� �ð� ��� �� ���� ä��� �ڷ�ƾ�� ����
    /// </summary>
    IEnumerator DecreaseRowRoutine()
    {
        for (int col = 0; col < _width; col++)
        {
            for (int row = 0; row < _height; row++)
            {
                // �� �����̸� �̵��� �� �ִ� ��ġ���� üũ
                bool isEmpty = _allFruits[col, row] == null;
                bool isNotBlankTile = !_tileManager.BlankTiles[col, row];
                bool isNoConcreteTile = _tileManager.ConcreteTiles[col, row] == null;
                bool isNoLavaTile = _tileManager.LavaTiles[col, row] == null;

                if (isEmpty && isNotBlankTile && isNoConcreteTile && isNoLavaTile)
                {
                    // �� ���� �Ʒ����� ���� ����� ������ ã�Ƽ� �� ĭ ����
                    for (int nextRow = row + 1; nextRow < _height; nextRow++)
                    {
                        if (_allFruits[col, nextRow] != null)
                        {
                            _allFruits[col, nextRow].Row = row;
                            _allFruits[col, nextRow] = null;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(_refillDelay * 0.5f);
        StartCoroutine(FillFruitRoutine());
    }


    /// <summary>
    /// ���忡 �� ������ ä���, �߰� ��ġ�� ���� ��� ��������� ��ġ ó��
    /// ��� Ÿ�� ���� �� ���� �̺�Ʈ ���� �� ���� ���¸� �̵� ���·� ����
    /// </summary>
    IEnumerator FillFruitRoutine()
    {
        yield return new WaitForSeconds(_refillDelay);

        // �� ������ ���� ä���
        RefillFruit();
        yield return new WaitForSeconds(_refillDelay);

        // ���忡 �߰� ��ġ�� �ִ��� �ݺ� �˻�
        while (MatchOnBoard())
        {
            yield return new WaitForSeconds(_refillDelay);

            // �޺� ī��Ʈ ����
            _streakValue++;

            // ��ġ�� ���� ó��
            CheckMatchFruit();
            yield break;
        }

        // �� �� �� ��� �� �߰� ��ġ �˻�
        yield return new WaitForSeconds(_refillDelay);

        if (MatchOnBoard())
        {
            _streakValue++;
            CheckMatchFruit();
            yield break;
        }

        _matchFinder.MatchFruits.Clear();
        _currentFruit = null;

        // �߰� ��� Ÿ�� ����
        _tileManager.CreateMoreLavaTiles();
        _eventManager.Shuffle.EventEffect();

        yield return new WaitForSeconds(_refillDelay);

        _gameManager.ChangeGameState(EGameStateType.Move);
        _tileManager.CheckCreateMoreLavaTile();
        _streakValue = 1;
    }


}
