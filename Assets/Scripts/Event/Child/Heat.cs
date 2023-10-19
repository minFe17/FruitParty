using System.Collections;
using UnityEngine;

public class Heat : Event
{
    protected override void Start()
    {
        base.Start();
        _eventType = EEventType.Heat;
        _eventManager.Events.Add(this);
        _eventUIManager.EventUI.TryGetValue(_eventType, out _eventUI);
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
        _eventUI.OnEventUI();
        yield return new WaitForSeconds(_eventDelay);

        Hot();
        _eventUI.OffEventUI();
        yield return new WaitForSeconds(_eventDelay);
        _gameManager.GameState = EGameStateType.Move;
    }
}