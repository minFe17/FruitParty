using System.Collections;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

public class FruitManager : MonoBehaviour
{
    // 싱글턴
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
        // 과일 매니저 초기화: 부모 Transform 지정 및 보드 크기 설정, 매니저 인스턴스 로드
        _parent = parent;
        _width = x;
        _height = y;
        _allFruits = new Fruit[x, y];
        LoadManagers();
    }

    void LoadManagers()
    {
        // 싱글턴 매니저들 인스턴스 가져오기
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
    /// 매치된 과일을 처리: 점수와 시간 증가, 타일 상태 체크, 매치 리스트에서 제거 후 과일 삭제
    /// </summary>
    void DestroyMatchFruit(int column, int row)
    {
        Fruit fruit = _allFruits[column, row];

        if (fruit != null && fruit.IsMatch)
        {
            PlayMatchAudio();

            // 해당 위치 타일 상태 체크
            _tileManager.CheckTile(column, row);

            _scoreManager.AddScore(_baseFruitScore * _streakValue); 
            _gameManager.AddTime(_streakValue);

            // 매치 리스트에서 과일 제거
            _matchFinder.MatchFruits.Remove(fruit); 
            DestroyFruit(fruit);                    
        }
    }

    void RefillFruit()
    {
        // 빈 공간에 새 과일 생성 (타일 상태 확인 후)
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
    /// 지정 위치에 매치되지 않는 과일 타입을 랜덤으로 선택하여 생성
    /// </summary>
    void SelectCreateFruit(int x, int y)
    {
        // 과일 타입 랜덤 선택, 기존 매치 방지 위해 반복 체크
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
    /// 현재 보드에 매치된 과일이 있는지 검사하고, 있으면 true를 반환
    /// </summary>
    bool MatchOnBoard()
    {
        // 보드 전체를 검사해 매치된 과일이 있는지 확인
        _matchFinder.FindAllMatch();  // 모든 위치에서 매치 과일 탐색 수행
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_allFruits[i, j] != null && _allFruits[i, j].IsMatch)
                    return true;  // 매치 과일 발견 시 즉시 true 반환
            }
        }
        return false;  // 매치 과일 없으면 false 반환
    }


    bool CheckColumnMatch(int column, int row, EFruitType type)
    {
        // 세로 방향으로 3매치가 있는지 검사
        if (_allFruits[column - 1, row] != null && _allFruits[column - 2, row] != null)
        {
            if (_allFruits[column - 1, row].FruitType == type && _allFruits[column - 2, row].FruitType == type)
                return true;
        }
        return false;
    }

    bool CheckRowMatch(int column, int row, EFruitType type)
    {
        // 가로 방향으로 3매치가 있는지 검사
        if (_allFruits[column, row - 1] != null && _allFruits[column, row - 2] != null)
        {
            if (_allFruits[column, row - 1].FruitType == type && _allFruits[column, row - 2].FruitType == type)
                return true;
        }
        return false;
    }

    void SwitchFruit(int column, int row, Vector2Int direction)
    {
        // 인접한 두 과일 위치를 교환
        if (_allFruits[column + direction.x, row + direction.y] != null)
        {
            Fruit temp = _allFruits[column + direction.x, row + direction.y];
            _allFruits[column + direction.x, row + direction.y] = _allFruits[column, row];
            _allFruits[column, row] = temp;
        }
    }

    /// <summary>
    /// 특정 위치에서 가로 3매치가 있는지 검사
    /// </summary>
    bool CheckHorizontalMatch(int x, int y)
    {
        if (x + 2 >= _width) return false;
        if (_allFruits[x + 1, y] == null || _allFruits[x + 2, y] == null) return false;

        Fruit[] fruits = { _allFruits[x, y], _allFruits[x + 1, y], _allFruits[x + 2, y] };
        return _matchFinder.CheckMatch(fruits);
    }

    /// <summary>
    /// 특정 위치에서 세로 3매치가 있는지 검사
    /// </summary>
    bool CheckVerticalMatch(int x, int y)
    {
        if (y + 2 >= _height) return false;
        if (_allFruits[x, y + 1] == null || _allFruits[x, y + 2] == null) return false;

        Fruit[] fruits = { _allFruits[x, y], _allFruits[x, y + 1], _allFruits[x, y + 2] };
        return _matchFinder.CheckMatch(fruits);
    }

    /// <summary>
    /// 전체 보드에 3매치 이상이 존재하는지 검사
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
        // 매치 효과음 재생
        _audioClipManager.PlayMatchSFX();
    }

    public void CreateFruit(Vector2Int position)
    {
        // 지정된 위치에 과일 생성 시도
        int x = position.x;
        int y = position.y;
        SelectCreateFruit(x, y);
    }

    public void MakeFruit(EFruitType type, int column, int row)
    {
        // 과일 객체 생성 및 초기 위치 설정
        Vector2Int position = new Vector2Int(column, row);
        Fruit fruit = _factoryManager.MakeObject<EFruitType, Fruit>(type, position);

        _allFruits[column, row] = fruit;
        fruit.transform.position = new Vector2(column, row);
        fruit.transform.parent = _parent;
    }

    public bool MatchAt(int column, int row, EFruitType type)
    {
        // 특정 위치에서 과일 생성 시 3매치 발생 여부 검사
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
        // 외부 요청에 의한 과일 제거 (예: 구매 등)
        DestroyFruit(_allFruits[column, row]);
    }

    /// <summary>
    /// 매치된 과일 처리: 폭탄 생성 조건 확인 후 매치 과일 제거 및 빈 공간으로 내리기 코루틴 실행
    /// </summary>
    public void CheckMatchFruit()
    {
        if (_matchFinder.MatchFruits.Count >= 4)
            _bombManager.CheckMakeBomb();

        RemoveMatchedFruits();
        StartCoroutine(DecreaseRowRoutine());
    }

    /// <summary>
    /// 매치된 과일을 제거하고 매치 리스트를 초기화
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
    /// 지정한 방향으로 과일 위치를 교환한 뒤, 매치가 발생하는지 검사
    /// 검사 후 원래 위치로 복구
    /// </summary>
    public bool SwitchAndCheck(int column, int row, Vector2Int direction)
    {
        // 위치 교환
        SwitchFruit(column, row, direction);

        // 매치 검사
        bool isMatch = CheckForMatch();

        // 위치 원복
        SwitchFruit(column, row, direction);
        return isMatch;
    }

    public void DestroyFruit(Fruit fruit)
    {
        // 과일 오브젝트 풀에 반환 및 보드 배열에서 제거
        _objectPoolManager.Push(fruit.FruitType, fruit.gameObject);
        _allFruits[fruit.Column, fruit.Row] = null;
    }

    public void DestroyAllFruit()
    {
        // 전체 과일 제거
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
    /// 보드에서 빈 공간을 찾아 위에 있는 과일을 아래로 이동
    /// 이동 후 일정 시간 대기 후 과일 채우기 코루틴을 시작
    /// </summary>
    IEnumerator DecreaseRowRoutine()
    {
        for (int col = 0; col < _width; col++)
        {
            for (int row = 0; row < _height; row++)
            {
                // 빈 공간이며 이동할 수 있는 위치인지 체크
                bool isEmpty = _allFruits[col, row] == null;
                bool isNotBlankTile = !_tileManager.BlankTiles[col, row];
                bool isNoConcreteTile = _tileManager.ConcreteTiles[col, row] == null;
                bool isNoLavaTile = _tileManager.LavaTiles[col, row] == null;

                if (isEmpty && isNotBlankTile && isNoConcreteTile && isNoLavaTile)
                {
                    // 빈 공간 아래에서 가장 가까운 과일을 찾아서 한 칸 내림
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
    /// 보드에 빈 공간을 채우고, 추가 매치가 있을 경우 재귀적으로 매치 처리
    /// 용암 타일 생성 및 셔플 이벤트 실행 후 게임 상태를 이동 상태로 변경
    /// </summary>
    IEnumerator FillFruitRoutine()
    {
        yield return new WaitForSeconds(_refillDelay);

        // 빈 공간에 과일 채우기
        RefillFruit();
        yield return new WaitForSeconds(_refillDelay);

        // 보드에 추가 매치가 있는지 반복 검사
        while (MatchOnBoard())
        {
            yield return new WaitForSeconds(_refillDelay);

            // 콤보 카운트 증가
            _streakValue++;

            // 매치된 과일 처리
            CheckMatchFruit();
            yield break;
        }

        // 한 번 더 대기 후 추가 매치 검사
        yield return new WaitForSeconds(_refillDelay);

        if (MatchOnBoard())
        {
            _streakValue++;
            CheckMatchFruit();
            yield break;
        }

        _matchFinder.MatchFruits.Clear();
        _currentFruit = null;

        // 추가 용암 타일 생성
        _tileManager.CreateMoreLavaTiles();
        _eventManager.Shuffle.EventEffect();

        yield return new WaitForSeconds(_refillDelay);

        _gameManager.ChangeGameState(EGameStateType.Move);
        _tileManager.CheckCreateMoreLavaTile();
        _streakValue = 1;
    }


}
