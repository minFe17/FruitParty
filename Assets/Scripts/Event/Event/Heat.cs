using System.Collections;
using UnityEngine;

public class Heat : Event
{
    protected override void Start()
    {
        base.Start();
        _eventType = EEventType.Heat;
        _eventManager.Events.Add(this);
    }

    public override void EventEffect()
    {
        _gameManager.GameState = EGameStateType.Event;
        StartCoroutine(HeatRoutine());
    }

    void Hot()
    {
        int creatableTiles = Random.Range(_minCreatableTiles, _maxCreatableTiles);
        for (int i = 0; i <= creatableTiles; i++)
        {
            _tileManager.CreateLockTiles();
        }
    }

    IEnumerator HeatRoutine()
    {
        // ui 이미지 보여주기
        yield return new WaitForSeconds(_eventDelay);
        Hot();
        // ui 이미지 숨기기
        yield return new WaitForSeconds(_eventDelay);
        _gameManager.GameState = EGameStateType.Move;
    }
}
