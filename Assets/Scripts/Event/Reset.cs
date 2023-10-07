using System.Collections;
using UnityEngine;

public class Reset : Event
{
    protected override void Start()
    {
        base.Start();
        _eventType = EEventType.Reset;
    }

    public override void EventEffect()
    {
        _gameManager.GameState = EGameStateType.Event;
        StartCoroutine(ResetRoutine());
    }
    
    IEnumerator ResetRoutine()
    {
        // ui ���� �̹��� �����ֱ�
        yield return new WaitForSeconds(0.5f);
        _tileManager.ResetTile();
        // ui ���� �̹��� �����
        yield return new WaitForSeconds(0.5f);
        _gameManager.GameState = EGameStateType.Move;
    }
}
