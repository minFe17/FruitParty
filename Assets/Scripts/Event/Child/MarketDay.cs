using System.Collections;
using UnityEngine;

public class MarketDay : Event, IEvent
{
    int _minBuyFruits = 3;
    int _maxBuyFruits = 11;

    protected override void Start()
    {
        base.Start();
        _eventType = EEventType.MarketDay;
        _eventManager.Events.Add(this);
        _eventUIManager.EventUI.TryGetValue(_eventType, out _eventUI);
    }

    void IEvent.EventEffect()
    {
        _gameManager.ChangeGameState(EGameStateType.Event);
        StartCoroutine(MarketDayRoutine());
    }

    void Market()
    {
        int creatableTiles = Random.Range(_minCreatableTiles, _maxCreatableTiles);
        for (int i = 0; i <= creatableTiles; i++)
            _tileManager.CreateBlankTiles();
    }

    void BuyFruit()
    {
        int buyFruit = Random.Range(_minBuyFruits, _maxBuyFruits);
        int column;
        int row;
        for (int i = 0; i < buyFruit; i++)
        {
            CalculatePosition(out column, out row);
            _fruitManager.BuyFruit(column, row);
        }
        _fruitManager.CheckMatchFruit();
    }

    void CalculatePosition(out int column, out int row)
    {
        column = Random.Range(0, _width);
        row = Random.Range(0, _height);
        bool isBuyPosition = false;
        while(!isBuyPosition)
        {
            if (_fruitManager.AllFruits[column, row] != null)
            {
                break;
            }
            else
            {
                column = Random.Range(0, _width);
                row = Random.Range(0, _height);
            }
        }
    }

    IEnumerator MarketDayRoutine()
    {
        _eventUI.OnEventUI();
        yield return new WaitForSeconds(_eventUIDelay);

        Market();
        yield return new WaitForSeconds(_eventDelay);
        BuyFruit();
        yield return new WaitForSeconds(_eventDelay);

        _eventUI.OffEventUI();
        yield return new WaitForSeconds(_eventUIDelay);
        _gameManager.ChangeGameState(EGameStateType.Move);
    }
}