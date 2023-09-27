using System.Collections;
using UnityEngine;
using Utils;

public class Fruit : MonoBehaviour
{
    [SerializeField] EFruitType _fruitType;
    [SerializeField] protected EColorType _color;
    [SerializeField] float _moveSpeed;

    protected EBombType _bombType;
    protected MatchFinder _matchFinder;
    protected Fruit _otherFruit;

    FruitManager _fruitManager;
    BombManager _bombManager;
    HintManager _hintManager;
    GameManager _gameManager;
    GameObject _destroyEffect;

    Vector2 _firstTouchPos = Vector2.zero;
    Vector2 _finalTouchPos = Vector2.zero;
    Vector2 _position;

    protected int _column;
    protected int _row;
    protected bool _isMatch;
    protected bool _isBomb;

    int _previousColumn;
    int _previousRow;
    int _targetX;
    int _targetY;
    float _swipeAngle;
    float _swipeResist = 1f;
    bool _onEffect;

    public EFruitType FruitType { get => _fruitType; }
    public EColorType ColorType { get => _color; }
    public EBombType BombType { get => _bombType; }
    public Fruit OtherFruit { get => _otherFruit; set => _otherFruit = value; }
    public int Column { get => _column; set => _column = value; }
    public int Row { get => _row; set => _row = value; }
    public bool IsMatch { get => _isMatch; set => _isMatch = value; }
    public bool IsBomb { get => _isBomb; }

    protected virtual void Awake()
    {
        _matchFinder = GenericSingleton<MatchFinder>.Instance;
        _fruitManager = GenericSingleton<FruitManager>.Instance;
        _bombManager = GenericSingleton<BombManager>.Instance;
        _hintManager = GenericSingleton<HintManager>.Instance;
        _gameManager = GenericSingleton<GameManager>.Instance;
        _destroyEffect = Resources.Load("Prefabs/Effect/DestroyEffect") as GameObject;
    }

    protected virtual void Update()
    {
        MoveFruit();
        MatchFruit();
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
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            sprite.color = new Color(0.5f, 0.5f, 0.5f, 1);
            if (!_onEffect)
            {
                Instantiate(_destroyEffect, transform.position, Quaternion.identity);
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
        if (_swipeAngle > -45 && _swipeAngle <= 45 && _column < _fruitManager.Width)    //Right
            RealMoveFruit(Vector2Int.right);
        else if (_swipeAngle > 45 && _swipeAngle <= 135 && _row < _fruitManager.Height) //Up
            RealMoveFruit(Vector2Int.up);
        else if (_swipeAngle > 135 || _swipeAngle <= -135 && _column > 0)               //Left
            RealMoveFruit(Vector2Int.left);
        else if (_swipeAngle < -45 && _swipeAngle >= -135 && _row > 0)                  //Down
            RealMoveFruit(Vector2Int.down);
        else
            _gameManager.GameState = EGameStateType.Move;
    }

    void RealMoveFruit(Vector2Int direction)
    {
        _otherFruit = _fruitManager.AllFruits[_column + direction.x, _row + direction.y];
        _previousColumn = _column;
        _previousRow = _row;
        if (_fruitManager.Board.LockTiles[_column, _row] == null && _fruitManager.Board.LockTiles[_column + direction.x, _row + direction.y] == null) //TileManager 스크립트 구현후 제거 예정 (_fruitManager.Board 대신 TileManager에 있는 LockTiles 속성 사용)
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

    void MakeBomb(GameObject bombGameObject)
    {
        Fruit bomb = bombGameObject.GetComponent<Fruit>();
        GenericSingleton<FruitManager>.Instance.AllFruits[_column, _row] = bomb;
        bomb.Column = _column;
        bomb.Row = _row;
        _matchFinder.MatchFruits.Clear();
        Destroy(this.gameObject);
    }

    public void MakeLineBomb()
    {
        GameObject temp = Instantiate(_bombManager.LineBombs[(int)_color], transform.position, Quaternion.identity);
        MakeBomb(temp);
    }

    public void MakeSquareBomb()
    {
        GameObject temp = Instantiate(_bombManager.SquareBombs[(int)_color], new Vector2(_column, _row), Quaternion.identity);
        MakeBomb(temp);
    }

    public void MakeFruitBomb()
    {
        GameObject temp = Instantiate(_bombManager.FruitBomb, transform.position, Quaternion.identity);
        MakeBomb(temp);
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
