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
                        _fruitManager.DestroyFruit(_fruitManager.AllFruits[x, y]);
                        continue;
                    }
                    else if (!_tileManager.BlankTiles[newX, y] && _tileManager.ConcreteTiles[newX, y] == null && _tileManager.LavaTiles[newX, y] == null)
                    {
                        _fruitManager.AllFruits[x, y].Column = newX;
                        _fruitManager.AllFruits[newX, y] = _fruitManager.AllFruits[x, y]; 
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
                    Vector2Int position = new Vector2Int(x, y);
                    _fruitManager.CreateFruit(position);
                }
            }
        }
    }

    IEnumerator TyphoonRoutine()
    {
        _eventUI.OnEventUI();
        yield return new WaitForSeconds(_eventUIDelay);

        ColdWind();
        MoveFruit();
        yield return new WaitForSeconds(_eventDelay);
        _fruitManager.CheckMatchFruit();
        yield return new WaitForSeconds(_eventDelay);

        _eventUI.OffEventUI();
        yield return new WaitForSeconds(_eventUIDelay);
        _gameManager.GameState = EGameStateType.Move;
    }
}