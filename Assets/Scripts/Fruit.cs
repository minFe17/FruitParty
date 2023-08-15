using System.Collections;
using UnityEngine;
using Utils;

public class Fruit : MonoBehaviour
{
    [SerializeField] EFruitType _fruitType;
    [SerializeField] float _moveSpeed;

    GameObject _otherFruit;
    FruitManager _fruitManager;

    Vector2 _firstTouchPos;
    Vector2 _finalTouchPos;
    Vector2 _position;

    int _column;
    int _row;
    int _previousColumn;
    int _previousRow;
    int _targetX;
    int _targetY;
    float _swipeAngle;
    bool _isMatch;

    public EFruitType FruitType { get => _fruitType; }
    public int Column { get => _column; set => _column = value; }
    public int Row { get => _row; set => _row = value; }
    public bool IsMatch { get => _isMatch; set => _isMatch = value; }


    void Start()
    {
        _fruitManager = GenericSingleton<FruitManager>.Instance;
        _targetX = (int)transform.position.x;
        _targetY = (int)transform.position.y;
        _column = _targetX;
        _row = _targetY;
        _previousColumn = _column;
        _previousRow = _row;
    }

    void Update()
    {
        FindMatchs();
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
        }
        else
        {
            _position = new Vector2(_targetX, transform.position.y);
            transform.position = _position;
            _fruitManager.AllFruits[_column, _row] = this.gameObject;
        }
        if (Mathf.Abs(_targetY - transform.position.y) > 0.1f)
        {
            _position = new Vector2(transform.position.x, _targetY);
            transform.position = Vector2.Lerp(transform.position, _position, _moveSpeed);
        }
        else
        {
            _position = new Vector2(transform.position.x, _targetY);
            transform.position = _position;
            _fruitManager.AllFruits[_column, _row] = this.gameObject;
        }
    }

    void MatchFruit()
    {
        if (_isMatch)
        {
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            sprite.color = new Color(0f, 0f, 0f, 1);
        }
    }

    void CalculateAngle()
    {
        float x = _finalTouchPos.x - _firstTouchPos.x;
        float y = _finalTouchPos.y - _firstTouchPos.y;
        _swipeAngle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
        SeleteMoveFruit();
    }

    void SeleteMoveFruit()
    {
        if (_swipeAngle > -45 && _swipeAngle <= 45 && _column < _fruitManager.Width)           //Right
        {
            _otherFruit = _fruitManager.AllFruits[_column + 1, _row];
            _otherFruit.GetComponent<Fruit>().Column -= 1;
            _column += 1;
        }
        else if (_swipeAngle > 45 && _swipeAngle <= 135 && _row < _fruitManager.Height)  //Up
        {
            _otherFruit = _fruitManager.AllFruits[_column, _row + 1];
            _otherFruit.GetComponent<Fruit>().Row -= 1;
            _row += 1;
        }
        else if (_swipeAngle > 135 || _swipeAngle <= -135 && _column > 0)              //Left
        {
            _otherFruit = _fruitManager.AllFruits[_column - 1, _row];
            _otherFruit.GetComponent<Fruit>().Column += 1;
            _column -= 1;
        }
        else if (_swipeAngle < -45 && _swipeAngle >= -135 && _row > 0)           //Down
        {
            _otherFruit = _fruitManager.AllFruits[_column, _row - 1];
            _otherFruit.GetComponent<Fruit>().Row += 1;
            _row -= 1;
        }

        StartCoroutine(CheckMoveRoutine());
    }

    void FindMatchs()
    {
        if (_column > 0 && _column < _fruitManager.Width - 1)
        {
            Fruit leftFruit = _fruitManager.AllFruits[_column - 1, _row].GetComponent<Fruit>();
            Fruit rightFrite = _fruitManager.AllFruits[_column + 1, _row].GetComponent<Fruit>();
            if (leftFruit.FruitType == _fruitType && rightFrite.FruitType == _fruitType)
            {
                leftFruit.IsMatch = true;
                rightFrite.IsMatch = true;
                _isMatch = true;
            }
        }
        if (_row > 0 && _row < _fruitManager.Height - 1)
        {
            Fruit upFruit = _fruitManager.AllFruits[_column, _row + 1].GetComponent<Fruit>();
            Fruit downFrite = _fruitManager.AllFruits[_column, _row - 1].GetComponent<Fruit>();
            if (upFruit.FruitType == _fruitType && downFrite.FruitType == _fruitType)
            {
                upFruit.IsMatch = true;
                downFrite.IsMatch = true;
                _isMatch = true;
            }
        }
    }

    public IEnumerator CheckMoveRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        if (_otherFruit != null)
        {
            if (!_isMatch && !_otherFruit.GetComponent<Fruit>().IsMatch)
            {
                _otherFruit.GetComponent<Fruit>().Column = _column;
                _otherFruit.GetComponent<Fruit>().Row = _row;
                _column = _previousColumn;
                _row = _previousRow;
            }
        }
    }

    private void OnMouseDown()
    {
        _firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        _finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
    }
}
