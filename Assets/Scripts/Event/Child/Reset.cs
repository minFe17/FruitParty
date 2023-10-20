using System.Collections;
using UnityEngine;

public class Reset : Event
{
    protected override void Start()
    {
        base.Start();
        _eventType = EEventType.Reset;
        _eventManager.Reset = this;
        _eventUIManager.EventUI.TryGetValue(_eventType, out _eventUI);
    }

    public override void EventEffect()
    {
        _gameManager.GameState = EGameStateType.Event;
        StartCoroutine(ResetRoutine());
    }
    
    IEnumerator ResetRoutine()
    {
        _eventUI.OnEventUI();
        yield return new WaitForSeconds(_eventUIDelay);

        _tileManager.ResetTile();
        yield return new WaitForSeconds(_eventDelay);

        _eventUI.OffEventUI();
        yield return new WaitForSeconds(_eventUIDelay);
        _gameManager.GameState = EGameStateType.Move;
    }
}
