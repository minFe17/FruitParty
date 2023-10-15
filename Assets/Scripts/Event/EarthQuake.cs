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
        // ui 이미지 보여주기
        yield return new WaitForSeconds(0.5f);
        Quake();

        yield return new WaitForSeconds(0.5f);
        _shuffle.ShuffleFruit();

        while (!_shuffle.EndShuffle)
        {
            yield return new WaitForSeconds(0.1f);
            if (_shuffle.EndShuffle)
                break;
        }
        // ui 이미지 숨기기
        yield return new WaitForSeconds(0.5f);
        _gameManager.GameState = EGameStateType.Move;
    }
}