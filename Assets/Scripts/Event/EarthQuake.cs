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
        _shuffle = GenericSingleton<EventManager>.Instance.Shuffle;
    }

    public override void EventEffect()
    {
        _gameManager.GameState = EGameStateType.Event;
        StartCoroutine(EarthQuakeRoutine());
    }

    void Quake()
    {
        int creatableTiles = Random.Range(0, _maxCreatableTiles);
        for (int i = 0; i < creatableTiles; i++)
        {
            _tileManager.CreateConcreteTiles();
        }
        _shuffle.ShuffleFruit();
    }

    IEnumerator EarthQuakeRoutine()
    {
        // ui 리셋 이미지 보여주기
        yield return new WaitForSeconds(0.5f);
        Quake();

        while (!_shuffle.EndShuffle)
        {
            yield return new WaitForSeconds(0.1f);
            if (_shuffle.EndShuffle)
                break;
        }
        // ui 리셋 이미지 숨기기
        yield return new WaitForSeconds(0.5f);
        _gameManager.GameState = EGameStateType.Move;
    }
}