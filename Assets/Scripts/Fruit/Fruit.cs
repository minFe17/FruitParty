using System.Collections;
using UnityEngine;
using Utils;

/// <summary>
/// 개별 과일(Fruit) 클래스
/// - 위치, 타입, 색상, 폭탄 여부 등 상태 관리
/// - 스와이프 입력 받아 이동 처리 및 매치 효과 표시
/// - 매치되면 효과 생성 및 파괴 처리 담당
/// </summary>
public class Fruit : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] EFruitType _fruitType;
    [SerializeField] protected EColorType _colorType;
    [SerializeField] float _moveSpeed;

    protected EBombType _bombType;
    protected FruitManager _fruitManager;
    protected FactoryManager _factoryManager;
    protected MatchFinder _matchFinder;
    protected BombManager _bombManager;
    protected SpriteManager _spriteManager;
    protected Fruit _otherFruit;

    protected int _column;
    protected int _row;
    protected bool _isMatch;
    protected bool _isBomb;

    GameManager _gameManager;
    TileManager _tileManager;

    Vector2 _firstTouchPos = Vector2.zero;
    Vector2 _finalTouchPos = Vector2.zero;
    Vector2 _position;

    int _previousColumn;
    int _previousRow;
    int _targetX;
    int _targetY;
    float _swipeAngle;
    float _swipeResist = 1f;
    bool _onEffect;

    public EFruitType FruitType { get => _fruitType; }
    public EColorType ColorType { get => _colorType; set => _colorType = value; }
    public EBombType BombType { get => _bombType; }
    public Fruit OtherFruit { get => _otherFruit; set => _otherFruit = value; }
    public int Column { get => _column; set => _column = value; }
    public int Row { get => _row; set => _row = value; }
    public bool IsMatch { get => _isMatch; set => _isMatch = value; }
    public bool IsBomb { get => _isBomb; }

    protected virtual void Awake()
    {
        SetManager();
    }

    void OnEnable()
    {
        Init();
    }

    protected virtual void Update()
    {
        SmoothMoveToTargetPosition();    // 목표 위치로 부드럽게 이동
        MatchFruit();   // 매치시 효과 처리
        DestroyFruit(); // 위치 불일치시 제거
    }

    protected virtual void SetSprite()
    {
        string fruit = _fruitType.ToString();
        _spriteRenderer.sprite = _spriteManager.FruitAtlas.GetSprite(fruit);
    }

    public virtual void Init()
    {
        _isMatch = false;
        _onEffect = false;
        SetSprite();
        _spriteRenderer.color = new Color(1f, 1f, 1f, 1);
    }

    /// <summary>
    /// 싱글턴 매니저 인스턴스 가져오기
    /// </summary>
    void SetManager()
    {
        _factoryManager = GenericSingleton<FactoryManager>.Instance;
        _fruitManager = GenericSingleton<FruitManager>.Instance;
        _matchFinder = GenericSingleton<MatchFinder>.Instance;
        _bombManager = GenericSingleton<BombManager>.Instance;
        _spriteManager = GenericSingleton<SpriteManager>.Instance;
        _gameManager = GenericSingleton<GameManager>.Instance;
        _tileManager = GenericSingleton<TileManager>.Instance;
    }

    /// <summary>
    /// 현재 행과 열 위치에 맞춰 과일을 부드럽게 이동시킴
    /// </summary>
    void SmoothMoveToTargetPosition()
    {
        Vector2 targetPosition = new Vector2(_column, _row);

        // 목표 위치와 현재 위치가 어느 정도 차이나면 부드럽게 이동
        if (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector2.Lerp(transform.position, targetPosition, _moveSpeed);

            // 위치가 바뀌면 FruitManager 배열 갱신 및 매치 검사 호출
            if (_fruitManager.AllFruits[_column, _row] != this)
            {
                _fruitManager.AllFruits[_column, _row] = this;
                _matchFinder.FindAllMatch();
            }
        }
        else
        {
            // 목표 위치에 거의 도달하면 정확히 위치 고정
            transform.position = targetPosition;
        }
    }

    void Move()
    {
        transform.position = Vector2.Lerp(transform.position, _position, _moveSpeed);
        if (_fruitManager.AllFruits[_column, _row] != this)
        {
            _fruitManager.AllFruits[_column, _row] = this;

            // 위치 변경 후 전체 매치 검사
            _matchFinder.FindAllMatch(); 
        }
    }

    /// <summary>
    /// 매치된 과일은 색상 변화 및 파괴 이펙트 생성
    /// </summary>
    void MatchFruit()
    {
        if (_isMatch)
        {
            _spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 1);
            if (!_onEffect)
            {
                Vector2Int position = new Vector2Int(_column, _row);
                // 팩토리 패턴으로 이펙트 생성
                _factoryManager.MakeObject<EEffectType, GameObject>(EEffectType.Destroy, position); 
                _onEffect = true;
            }
        }
    }

    /// <summary>
    /// 터치 끝 좌표로 스와이프 각도 계산 후 이동 방향 결정
    /// </summary>
    void CalculateAngle()
    {
        float x = _finalTouchPos.x - _firstTouchPos.x;
        float y = _finalTouchPos.y - _firstTouchPos.y;
        if (Mathf.Abs(x) > _swipeResist || Mathf.Abs(y) > _swipeResist)
        {
            _gameManager.ChangeGameState(EGameStateType.Wait);
            _swipeAngle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
            SeleteMoveFruit();
            _fruitManager.CurrentFruit = this;
        }
        else
            _gameManager.ChangeGameState(EGameStateType.Move);
    }

    /// <summary>
    /// 스와이프 각도에 따른 이동 방향 판정 및 이동 처리
    /// </summary>
    void SeleteMoveFruit()
    {
        if (_swipeAngle > -45 && _swipeAngle <= 45 && _column < _fruitManager.Width)    // Right
            TrySwapWithAdjacentFruit(Vector2Int.right);
        else if (_swipeAngle > 45 && _swipeAngle <= 135 && _row < _fruitManager.Height) // Up
            TrySwapWithAdjacentFruit(Vector2Int.up);
        else if (_swipeAngle > 135 || _swipeAngle <= -135 && _column > 0)               // Left
            TrySwapWithAdjacentFruit(Vector2Int.left);
        else if (_swipeAngle < -45 && _swipeAngle >= -135 && _row > 0)                  // Down
            TrySwapWithAdjacentFruit(Vector2Int.down);
        else
            _gameManager.ChangeGameState(EGameStateType.Move);
    }

    /// <summary>
    /// 지정된 방향의 인접 과일과 위치를 교환 시도하고 이동 결과를 검사함
    /// </summary>
    void TrySwapWithAdjacentFruit(Vector2Int direction)
    {
        int newX = _column + direction.x;
        int newY = _row + direction.y;

        // 보드 범위 검사
        if (newX < 0 || newX >= _fruitManager.Width || newY < 0 || newY >= _fruitManager.Height)
        {
            _gameManager.ChangeGameState(EGameStateType.Move);
            return;
        }

        // 인접 과일 가져오기
        _otherFruit = _fruitManager.AllFruits[newX, newY];
        _previousColumn = _column;
        _previousRow = _row;

        // 잠금 타일 여부 확인
        if (_tileManager.LockTiles[_column, _row] != null || _tileManager.LockTiles[newX, newY] != null)
        {
            _gameManager.ChangeGameState(EGameStateType.Move);
            return;
        }

        if (_otherFruit != null)
        {
            // 위치 값 교환
            _otherFruit.Column -= direction.x;
            _otherFruit.Row -= direction.y;
            _column += direction.x;
            _row += direction.y;

            // 이동 후 매치 체크 코루틴 실행
            StartCoroutine(CheckMoveRoutine());
        }
        else
            _gameManager.ChangeGameState(EGameStateType.Move);
    }


    /// <summary>
    /// 자신의 위치와 일치하지 않으면 과일 제거 요청
    /// </summary>
    void DestroyFruit()
    {
        if (_fruitManager.AllFruits[_column, _row] != this)
            _fruitManager.DestroyFruit(this);
    }

    void OnMouseDown()
    {
        if (_gameManager.GameState == EGameStateType.Move)
            _firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void OnMouseUp()
    {
        if (_gameManager.GameState == EGameStateType.Move)
        {
            _finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    /// <summary>
    /// 이동 시도 후 일정 시간 대기하여 매치 여부 판단 및 위치 복구 처리
    /// </summary>
    IEnumerator CheckMoveRoutine()
    {
        if (this.IsBomb && _bombType == EBombType.FruitBomb)
            _isMatch = true;
        else if (_otherFruit.IsBomb && _otherFruit.BombType == EBombType.FruitBomb)
            _otherFruit.IsMatch = true;

        yield return new WaitForSeconds(0.5f);

        if (_otherFruit != null)
        {
            // 매치 실패 시 위치 제자리로
            if (!_isMatch && !_otherFruit.IsMatch)
            {
                _otherFruit.Column = _column;
                _otherFruit.Row = _row;
                _column = _previousColumn;
                _row = _previousRow;

                yield return new WaitForSeconds(0.5f);
                _gameManager.ChangeGameState(EGameStateType.Move);
                _fruitManager.CurrentFruit = null;
            }
            else
            {
                // 매치 성공 시 후속 처리
                _fruitManager.CheckMatchFruit();
            }
            _otherFruit = null;
        }
    }
}