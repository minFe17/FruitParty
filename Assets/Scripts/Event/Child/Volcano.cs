using System.Collections;
using UnityEngine;
using Utils;

public class Volcano : Event, IEvent
{
    Shuffle _shuffle;

    protected override void Start()
    {
        base.Start();
        _eventType = EEventType.Volcano;
        _eventManager.Events.Add(this);
        _eventUIManager.EventUI.TryGetValue(_eventType, out _eventUI);
        _shuffle = (Shuffle)GenericSingleton<EventManager>.Instance.Shuffle;
    }

    void IEvent.EventEffect()
    {
        _gameManager.ChangeGameState(EGameStateType.Event);
        StartCoroutine(VolcanoRoutine());
    }

    void Eruption()
    {
        int creatableTiles = Random.Range(_minCreatableTiles, _maxCreatableTiles);
        _tileManager.FirstCreateLavaTile = true;
        for (int i = 0; i <= creatableTiles; i++)
            _tileManager.CreateLavaTiles();
        _tileManager.CreateMoreLavaTile = false;
        _fruitManager.CheckMatchFruit();
    }

    IEnumerator VolcanoRoutine()
    {
        _eventUI.OnEventUI();
        yield return new WaitForSeconds(_eventUIDelay);
        yield return new WaitForSeconds(_eventDelay);

        Eruption();
        yield return new WaitForSeconds(_eventDelay);
        _shuffle.ShuffleFruit();

        while (!_shuffle.EndShuffle)
        {
            yield return new WaitForSeconds(0.1f);
            if (_shuffle.EndShuffle)
                break;
        }
        _fruitManager.CheckMatchFruit();
        yield return new WaitForSeconds(_eventDelay);

        _eventUI.OffEventUI();
        yield return new WaitForSeconds(_eventUIDelay);

        if (_gameManager.GameState != EGameStateType.Pause)
            _gameManager.ChangeGameState(EGameStateType.Move);
    }
}