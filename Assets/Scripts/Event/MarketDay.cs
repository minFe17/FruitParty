using System.Collections;
using UnityEngine;

public class MarketDay : Event
{
    int _minBuyFruits = 5;
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

        int buyFruit = Random.Range(_minBuyFruits, _maxBuyFruits);
        for (int i = 0; i < buyFruit; i++)
        {
            BuyFruit();
        }
    }

    void BuyFruit()
    {
        int maxIteration = _width * _height;
        for(int i=0; i<maxIteration; i++)
        {
            int column = Random.Range(0, _width);
            int row = Random.Range(0, _height);

            if (_fruitManager.AllFruits[column, row] != null)
            {
                _fruitManager.BuyFruit(column, row);
                break;
            }
        }
    }

    IEnumerator MarketDayRoutine()
    {
        // ui 이미지 보여주기
        yield return new WaitForSeconds(0.5f);
        Market();
        // ui 이미지 숨기기
        yield return new WaitForSeconds(0.5f);
        _gameManager.GameState = EGameStateType.Move;
    }
}
