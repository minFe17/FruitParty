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

    void MoveFruit()
    {
        _targetX = _column;
        _targetY = _row;
        if (Mathf.Abs(_targetX - transform.position.x) > 0.1f)
        {
            _position = new Vector2(_targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, _position, _moveSpeed);
            if (_fruitManager.AllFruits[_column, _row] != this)
            {
                _fruitManager.AllFruits[_column, _row] = this;
                _matchFinder.FindAllMatch();
            }
        }
        else
        {
            _position = new Vector2(_targetX, transform.position.y);
            transform.position = _position;
        }

        if (Mathf.Abs(_targetY - transform.position.y) > 0.1f)
        {
            _position = new Vector2(transform.position.x, _targetY);
            transform.position = Vector2.Lerp(transform.position, _position, _moveSpeed);
            if (_fruitManager.AllFruits[_column, _row] != this)
            {
                _fruitManager.AllFruits[_column, _row] = this;
                _matchFinder.FindAllMatch();
            }
        }
        else
        {
            _position = new Vector2(transform.position.x, _targetY);
            transform.position = _position;
        }
    }

    void MatchFruit()
    {
        if (_isMatch)
        {
            _spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 1);
            if (!_onEffect)
            {
                Vector2Int position = new Vector2Int(Column, Row);
                _factoryManager.MakeObject<EEffectType, GameObject>(EEffectType.Destroy, position);
                _onEffect = true;
            }
        }
    }

    void CalculateAngle()
    {
        float x = _finalTouchPos.x - _firstTouchPos.x;
        float y = _finalTouchPos.y - _firstTouchPos.y;
        if (Mathf.Abs(x) > _swipeResist || Mathf.Abs(y) > _swipeResist)
        {
            _gameManager.GameState = EGameStateType.Wait;
            _swipeAngle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
            SeleteMoveFruit();
            _fruitManager.CurrentFruit = this;
        }
        else
            _gameManager.GameState = EGameStateType.Move;
    }

    void SeleteMoveFruit()
    {
        if (_swipeAngle > -45 && _swipeAngle <= 45 && _column < _fruitManager.Width)
            RealMoveFruit(Vector2Int.right);
        else if (_swipeAngle > 45 && _swipeAngle <= 135 && _row < _fruitManager.Height)
            RealMoveFruit(Vector2Int.up);
        else if (_swipeAngle > 135 || _swipeAngle <= -135 && _column > 0)
            RealMoveFruit(Vector2Int.left);
        else if (_swipeAngle < -45 && _swipeAngle >= -135 && _row > 0)
            RealMoveFruit(Vector2Int.down);
        else
            _gameManager.GameState = EGameStateType.Move;
    }

    void RealMoveFruit(Vector2Int direction)
    {
        int newX = _column + direction.x;
        int newY = _row + direction.y;
        if (newX >= 0 && newX < _fruitManager.Width && newY >= 0 && newY < _fruitManager.Height)
        {
            _otherFruit = _fruitManager.AllFruits[newX, newY];
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
                    _gameManager.GameState = EGameStateType.Move;
            }
            else
                _gameManager.GameState = EGameStateType.Move;
        }
        else
            _gameManager.GameState = EGameStateType.Move;
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
            if (!_isMatch && !_otherFruit.IsMatch)
            {
                _otherFruit.Column = _column;
                _otherFruit.Row = _row;
                _column = _previousColumn;
                _row = _previousRow;
                yield return new WaitForSeconds(0.5f);
                _gameManager.GameState = EGameStateType.Move;
                _fruitManager.CurrentFruit = null;
            }
            else
                _fruitManager.CheckMatchFruit();

            _otherFruit = null;
        }
    }
}