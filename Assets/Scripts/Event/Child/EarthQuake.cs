using System.Collections;
using UnityEngine;
using Utils;

public class EarthQuake : Event
{
    Shuffle _shuffle;

    protected override void Start()
    {
        base.Start();
        _eventType = EEventType.EarthQuake;
        _eventManager.Events.Add(this);
        _eventUIManager.EventUI.TryGetValue(_eventType, out _eventUI);
        _shuffle = GenericSingleton<EventManager>.Instance.Shuffle;
    }

    public override void EventEffect()
    {
        _gameManager.GameState = EGameStateType.Event;
        StartCoroutine(EarthQuakeRoutine());
    }

    void Quake()
    {
        int creatableTiles = Random.Range(_minCreatableTiles, _maxCreatableTiles);
        for (int i = 0; i <= creatableTiles; i++)
            _tileManager.CreateConcreteTiles();
        _fruitManager.CheckMatchFruit();
    }

    IEnumerator EarthQuakeRoutine()
    {
        _eventUI.OnEventUI();
        yield return new WaitForSeconds(_eventUIDelay);

        Quake();
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
        _gameManager.GameState = EGameStateType.Move;
    }
}