using System.Collections;
using UnityEngine;
using Utils;

public class Volcano : Event
{
    Shuffle _shuffle;

    protected override void Start()
    {
        base.Start();
        _eventType = EEventType.Volcano;
        _eventManager.Events.Add(this);
        _shuffle = GenericSingleton<EventManager>.Instance.Shuffle;
        _eventUIManager.EventUI.TryGetValue(_eventType, out _eventUI);
    }

    public override void EventEffect()
    {
        _gameManager.GameState = EGameStateType.Event;
        StartCoroutine(VolcanoRoutine());
    }

    void Eruption()
    {
        int creatableTiles = Random.Range(_minCreatableTiles, _maxCreatableTiles);
        for (int i = 0; i <= creatableTiles; i++)
            _tileManager.CreateLavaTiles();
        _fruitManager.CheckMatchFruit();
    }

    IEnumerator VolcanoRoutine()
    {
        _eventUI.OnEventUI();
        yield return new WaitForSeconds(0.5f);

        Eruption();
        yield return new WaitForSeconds(0.5f);
        _shuffle.ShuffleFruit();

        while (!_shuffle.EndShuffle)
        {
            yield return new WaitForSeconds(0.1f);
            if (_shuffle.EndShuffle)
                break;
        }

        _eventUI.OffEventUI();
        yield return new WaitForSeconds(0.5f);
        _gameManager.GameState = EGameStateType.Move;
    }
}
