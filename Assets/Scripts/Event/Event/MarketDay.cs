using System.Collections;
using UnityEngine;

public class MarketDay : Event
{
    int _minBuyFruits = 3;
    int _maxBuyFruits = 11;

    protected override void Start()
    {
        base.Start();
        _eventType = EEventType.MarketDay;
        _eventManager.Events.Add(this);
    }

    public override void EventEffect()
    {
        _gameManager.GameState = EGameStateType.Event;
        StartCoroutine(MarketDayRoutine());
    }

    void Market()
    {
        int creatableTiles = Random.Range(_minCreatableTiles, _maxCreatableTiles);
        for (int i = 0; i <= creatableTiles; i++)
        {
            _tileManager.CreateBlankTiles();
        }
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
                isBuyPosition = true;
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
        // ui 이미지 보여주기
        yield return new WaitForSeconds(_eventDelay);
        Market();

        yield return new WaitForSeconds(_eventDelay);
        BuyFruit();

        // ui 이미지 숨기기
        yield return new WaitForSeconds(_eventDelay);
        _gameManager.GameState = EGameStateType.Move;
    }
}
