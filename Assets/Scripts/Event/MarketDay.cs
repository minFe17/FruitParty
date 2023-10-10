using System.Collections;
using UnityEngine;

public class MarketDay : Event
{
    int _maxBuyFruits = 10;

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
        int creatableTiles = Random.Range(0, _maxCreatableTiles);
        for (int i = 0; i < creatableTiles; i++)
        {
            _tileManager.CreateBlankTiles();
        }

        int buyFruit = Random.Range(0, _maxBuyFruits);
        for (int i = 0; i < buyFruit; i++)
        {
            BuyFruit();
        }
    }

    void BuyFruit()
    {
        int column = Random.Range(0, _width);
        int row = Random.Range(0, _height);

        Debug.Log(column);
        Debug.Log(row);
        if (_fruitManager.AllFruits[column, row] == null)
            BuyFruit();
        else
            _fruitManager.BuyFruit(column, row);
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
