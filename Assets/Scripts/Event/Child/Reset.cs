using System.Collections;
using UnityEngine;

public class Reset : Event, IEvent
{
    protected override void Start()
    {
        base.Start();
        _eventType = EEventType.Reset;
        _eventManager.Reset = this;
        _eventUIManager.EventUI.TryGetValue(_eventType, out _eventUI);
    }

    void IEvent.EventEffect()
    {
        _gameManager.ChangeGameState(EGameStateType.Event);
        StartCoroutine(ResetRoutine());
    }
    
    IEnumerator ResetRoutine()
    {
        _eventUI.OnEventUI();
        yield return new WaitForSeconds(_eventUIDelay);

        _tileManager.ResetTile();
        yield return new WaitForSeconds(_eventDelay);
        _fruitManager.CheckMatchFruit();
        yield return new WaitForSeconds(_eventDelay);

        _eventUI.OffEventUI();
        yield return new WaitForSeconds(_eventUIDelay);
        _gameManager.ChangeGameState(EGameStateType.Move);
    }
}