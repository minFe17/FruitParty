using System.Collections;
using UnityEngine;
using Utils;

/// <summary>
/// ���� ����(Fruit) Ŭ����
/// - ��ġ, Ÿ��, ����, ��ź ���� �� ���� ����
/// - �������� �Է� �޾� �̵� ó�� �� ��ġ ȿ�� ǥ��
/// - ��ġ�Ǹ� ȿ�� ���� �� �ı� ó�� ���
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
        SmoothMoveToTargetPosition();    // ��ǥ ��ġ�� �ε巴�� �̵�
        MatchFruit();   // ��ġ�� ȿ�� ó��
        DestroyFruit(); // ��ġ ����ġ�� ����
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
    /// �̱��� �Ŵ��� �ν��Ͻ� ��������
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
    /// ���� ��� �� ��ġ�� ���� ������ �ε巴�� �̵���Ŵ
    /// </summary>
    void SmoothMoveToTargetPosition()
    {
        Vector2 targetPosition = new Vector2(_column, _row);

        // ��ǥ ��ġ�� ���� ��ġ�� ��� ���� ���̳��� �ε巴�� �̵�
        if (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector2.Lerp(transform.position, targetPosition, _moveSpeed);

            // ��ġ�� �ٲ�� FruitManager �迭 ���� �� ��ġ �˻� ȣ��
            if (_fruitManager.AllFruits[_column, _row] != this)
            {
                _fruitManager.AllFruits[_column, _row] = this;
                _matchFinder.FindAllMatch();
            }
        }
        else
        {
            // ��ǥ ��ġ�� ���� �����ϸ� ��Ȯ�� ��ġ ����
            transform.position = targetPosition;
        }
    }

    void Move()
    {
        transform.position = Vector2.Lerp(transform.position, _position, _moveSpeed);
        if (_fruitManager.AllFruits[_column, _row] != this)
        {
            _fruitManager.AllFruits[_column, _row] = this;

            // ��ġ ���� �� ��ü ��ġ �˻�
            _matchFinder.FindAllMatch(); 
        }
    }

    /// <summary>
    /// ��ġ�� ������ ���� ��ȭ �� �ı� ����Ʈ ����
    /// </summary>
    void MatchFruit()
    {
        if (_isMatch)
        {
            _spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 1);
            if (!_onEffect)
            {
                Vector2Int position = new Vector2Int(_column, _row);
                // ���丮 �������� ����Ʈ ����
                _factoryManager.MakeObject<EEffectType, GameObject>(EEffectType.Destroy, position); 
                _onEffect = true;
            }
        }
    }

    /// <summary>
    /// ��ġ �� ��ǥ�� �������� ���� ��� �� �̵� ���� ����
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
    /// �������� ������ ���� �̵� ���� ���� �� �̵� ó��
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
    /// ������ ������ ���� ���ϰ� ��ġ�� ��ȯ �õ��ϰ� �̵� ����� �˻���
    /// </summary>
    void TrySwapWithAdjacentFruit(Vector2Int direction)
    {
        int newX = _column + direction.x;
        int newY = _row + direction.y;

        // ���� ���� �˻�
        if (newX < 0 || newX >= _fruitManager.Width || newY < 0 || newY >= _fruitManager.Height)
        {
            _gameManager.ChangeGameState(EGameStateType.Move);
            return;
        }

        // ���� ���� ��������
        _otherFruit = _fruitManager.AllFruits[newX, newY];
        _previousColumn = _column;
        _previousRow = _row;

        // ��� Ÿ�� ���� Ȯ��
        if (_tileManager.LockTiles[_column, _row] != null || _tileManager.LockTiles[newX, newY] != null)
        {
            _gameManager.ChangeGameState(EGameStateType.Move);
            return;
        }

        if (_otherFruit != null)
        {
            // ��ġ �� ��ȯ
            _otherFruit.Column -= direction.x;
            _otherFruit.Row -= direction.y;
            _column += direction.x;
            _row += direction.y;

            // �̵� �� ��ġ üũ �ڷ�ƾ ����
            StartCoroutine(CheckMoveRoutine());
        }
        else
            _gameManager.ChangeGameState(EGameStateType.Move);
    }


    /// <summary>
    /// �ڽ��� ��ġ�� ��ġ���� ������ ���� ���� ��û
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
    /// �̵� �õ� �� ���� �ð� ����Ͽ� ��ġ ���� �Ǵ� �� ��ġ ���� ó��
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
            // ��ġ ���� �� ��ġ ���ڸ���
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
                // ��ġ ���� �� �ļ� ó��
                _fruitManager.CheckMatchFruit();
            }
            _otherFruit = null;
        }
    }
}