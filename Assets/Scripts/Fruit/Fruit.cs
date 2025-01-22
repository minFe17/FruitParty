using System.Collections;
using UnityEngine;
using Utils;

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

    HintManager _hintManager;
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
        MoveFruit();
        MatchFruit();
        DestroyFruit();
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

    void SetManager()
    {
        _factoryManager = GenericSingleton<FactoryManager>.Instance;
        _fruitManager = GenericSingleton<FruitManager>.Instance;
        _matchFinder = GenericSingleton<MatchFinder>.Instance;
        _bombManager = GenericSingleton<BombManager>.Instance;
        _spriteManager = GenericSingleton<SpriteManager>.Instance;
        _hintManager = GenericSingleton<HintManager>.Instance;
        _gameManager = GenericSingleton<GameManager>.Instance;
        _tileManager = GenericSingleton<TileManager>.Instance;
    }

    void MoveFruit()    // 목표 위치로 이동
    {
        _targetX = _column;
        _targetY = _row;

        _position = new Vector2(_targetX, transform.position.y);
        if (Mathf.Abs(_targetX - transform.position.x) > 0.1f)
            Move();
        else
            transform.position = _position;

        _position = new Vector2(transform.position.x, _targetY);
        if (Mathf.Abs(_targetY - transform.position.y) > 0.1f)
            Move();
        else
            transform.position = _position;
    }

    void Move() // 부드럽게 이동시키는 역할
    {
        transform.position = Vector2.Lerp(transform.position, _position, _moveSpeed);
        if (_fruitManager.AllFruits[_column, _row] != this)
        {
            _fruitManager.AllFruits[_column, _row] = this;
            _matchFinder.FindAllMatch();
        }
    }

    void MatchFruit()
    {
        if (_isMatch)
        {
            _spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 1);
            if (!_onEffect)
            {
                Vector2Int position = new Vector2Int(_column, _row);
                _factoryManager.MakeObject<EEffectType, GameObject>(EEffectType.Destroy, position); // Factory 패턴 일반화 사요ㅛㅇ
                _onEffect = true;
            }
        }
    }

    void CalculateAngle()   // 유저 Swipe 계산
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

    void SeleteMoveFruit()  // 이동방향 결정
    {
        if (_swipeAngle > -45 && _swipeAngle <= 45 && _column < _fruitManager.Width)    // Right
            RealMoveFruit(Vector2Int.right);
        else if (_swipeAngle > 45 && _swipeAngle <= 135 && _row < _fruitManager.Height) // Up
            RealMoveFruit(Vector2Int.up);
        else if (_swipeAngle > 135 || _swipeAngle <= -135 && _column > 0)               // Left
            RealMoveFruit(Vector2Int.left);
        else if (_swipeAngle < -45 && _swipeAngle >= -135 && _row > 0)                  // Down
            RealMoveFruit(Vector2Int.down);
        else
            _gameManager.ChangeGameState(EGameStateType.Move);
    }

    void RealMoveFruit(Vector2Int direction)
    {
        int newX = _column + direction.x;
        int newY = _row + direction.y;
        if (newX >= 0 && newX < _fruitManager.Width && newY >= 0 && newY < _fruitManager.Height)
        {
            _otherFruit = _fruitManager.AllFruits[newX, newY];  // 현재 과일과 위치가 바뀔 과일
            _previousColumn = _column;
            _previousRow = _row;
            if (_tileManager.LockTiles[_column, _row] == null && _tileManager.LockTiles[_column + direction.x, _row + direction.y] == null)
            {
                if (_otherFruit != null)
                {
                    _otherFruit.Column += -1 * direction.x;
                    _otherFruit.Row += -1 * direction.y;
                    _column += direction.x;
                    _row += direction.y;
                    StartCoroutine(CheckMoveRoutine());
                }
                else
                    _gameManager.ChangeGameState(EGameStateType.Move);
            }
            else
                _gameManager.ChangeGameState(EGameStateType.Move);
        }
        else
            _gameManager.ChangeGameState(EGameStateType.Move);
    }

    void DestroyFruit()
    {
        if (_fruitManager.AllFruits[_column, _row] != this)
            _fruitManager.DestroyFruit(this);
    }

    void OnMouseDown()
    {
        _hintManager.DestroyHint();
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

    IEnumerator CheckMoveRoutine()
    {
        if (this.IsBomb && _bombType == EBombType.FruitBomb)
            _isMatch = true;
        else if (_otherFruit.IsBomb && _otherFruit.BombType == EBombType.FruitBomb)
            _otherFruit.IsMatch = true;

        yield return new WaitForSeconds(0.5f);
        if (_otherFruit != null)
        {
            if (!_isMatch && !_otherFruit.IsMatch)  // 매치가 되지 않았을 때
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
                _fruitManager.CheckMatchFruit();

            _otherFruit = null;
        }
    }
}