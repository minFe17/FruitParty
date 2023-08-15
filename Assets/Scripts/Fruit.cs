using UnityEngine;
using Utils;

public class Fruit : MonoBehaviour
{
    [SerializeField] float _moveSpeed;

    GameObject _otherFruit;
    FruitManager _fruitManager;

    Vector2 _firstTouchPos;
    Vector2 _finalTouchPos;
    Vector2 _position;

    int _row;
    int _column;
    int _targetX;
    int _targetY;
    float _swipeAngle;

    public int Row { get => _row; set => _row = value; }
    public int Column { get => _column; set => _column = value; }


    void Start()
    {
        _fruitManager = GenericSingleton<FruitManager>.Instance;
        _targetX = (int)transform.position.x;
        _targetY = (int)transform.position.y;
        _row = _targetY;
        _column = _targetX;
    }

    void Update()
    {
        MoveFruit();
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
