using System.Collections;
using UnityEngine;

public class Typhoon : Event
{
    int _minMoveAmount = 1;

    protected override void Start()
    {
        base.Start();
        _eventType = EEventType.Typhoon;
        _eventManager.Events.Add(this);
        _eventUIManager.EventUI.TryGetValue(_eventType, out _eventUI);
    }

    public override void EventEffect()
    {
        _gameManager.GameState = EGameStateType.Event;
        StartCoroutine(TyphoonRoutine());
    }

    void ColdWind()
    {
        int creatableTiles = Random.Range(_minCreatableTiles, _maxCreatableTiles);
        for (int i = 0; i <= creatableTiles; i++)
            _tileManager.CreateIceTiles();
    }

    void MoveFruit()
    {
        for (int y = 0; y < _height; y++)
        {
            int moveAmount = Random.Range(_minMoveAmount, _width);
            for (int x = _width - 1; x >= 0; x--)
            {
                if (_fruitManager.AllFruits[x, y] != null)
                {
                    int newX = x + moveAmount;
                    if (newX >= _width)
                    {
                        Destroy(_fruitManager.AllFruits[x, y].gameObject);
                        _fruitManager.AllFruits[x, y] = null;
                        continue;
                    }
                    else if (!_tileManager.BlankTiles[newX, y] && _tileManager.ConcreteTiles[newX, y] == null && _tileManager.LavaTiles[newX, y] == null)
                    {
                        _fruitManager.AllFruits[x, y].Column = newX;
                        _fruitManager.AllFruits[x, y] = null;
                    }
                }
            }
            CreateFruit(moveAmount);
        }
    }

    void CreateFruit(int createAmount)
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < createAmount; x++)
            {
                if (_fruitManager.AllFruits[x, y] == null && !_tileManager.BlankTiles[x, y] && _tileManager.ConcreteTiles[x, y] == null && _tileManager.LavaTiles[x, y] == null)
                {
                    Vector2 position = new Vector2(x, y);
                    int fruitNumber = Random.Range(0, _fruitManager.Fruits.Count);
                    int iterations = 0;

                    while (_fruitManager.MatchAt(x, y, _fruitManager.Fruits[fruitNumber]) && iterations < 100)
                    {
                        iterations++;
                        fruitNumber = Random.Range(0, _fruitManager.Fruits.Count);
                    }
                    iterations = 0;

                    GameObject fruit = Instantiate(_fruitManager.Fruits[fruitNumber], position, Quaternion.identity);
                    _fruitManager.AllFruits[x, y] = fruit.GetComponent<Fruit>();
                    fruit.GetComponent<Fruit>().Column = x;
                    fruit.GetComponent<Fruit>().Row = y;
                }
            }
        }
    }

    IEnumerator TyphoonRoutine()
    {
        _eventUI.OnEventUI();
        yield return new WaitForSeconds(_eventDelay);

        ColdWind();
        yield return new WaitForSeconds(_eventDelay);
        MoveFruit();

        _eventUI.OffEventUI();
        yield return new WaitForSeconds(_eventDelay);
        _gameManager.GameState = EGameStateType.Move;
    }
}