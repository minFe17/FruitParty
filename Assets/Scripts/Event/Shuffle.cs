using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuffle : Event
{
    bool _endShuffle;
    public bool EndShuffle { get => _endShuffle; }

    protected override void Start()
    {
        base.Start();
        _eventType = EEventType.Shuffle;
        _eventManager.Shuffle = this;
    }

    public override void EventEffect()
    {
        _gameManager.GameState = EGameStateType.Event;
        StartCoroutine(ShuffleFruitRoutine());
    }

    public void ShuffleFruit()
    {
        _endShuffle = false;
        List<Fruit> newFruit = new List<Fruit>();

        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_fruitManager.AllFruits[i, j] != null)
                    newFruit.Add(_fruitManager.AllFruits[i, j]);
            }
        }

        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (!_tileManager.BlankTiles[i, j] && _tileManager.ConcreteTiles[i, j] == null && _tileManager.LavaTiles[i, j] == null)
                {
                    int fruitIndex = Random.Range(0, newFruit.Count);

                    int iterations = 0;
                    while (_fruitManager.MatchAt(i, j, newFruit[fruitIndex].gameObject) && iterations <= 100)
                    {
                        fruitIndex = Random.Range(0, newFruit.Count);
                        iterations++;
                    }

                    Fruit fruit = newFruit[fruitIndex];
                    fruit.Column = i;
                    fruit.Row = j;
                    _fruitManager.AllFruits[i, j] = newFruit[fruitIndex];
                    newFruit.Remove(newFruit[fruitIndex]);
                }
            }
        }
        if (_fruitManager.IsDeadlocked())
            ShuffleFruit();
        else
            _endShuffle = true;
    }

    IEnumerator ShuffleFruitRoutine()
    {
        //이미지 보이기
        yield return new WaitForSeconds(0.5f);
        ShuffleFruit();
        while(!_endShuffle)
        {
            yield return new WaitForSeconds(0.1f);
            if (_endShuffle)
                break;
        }
        //이미지 숨기기
        yield return new WaitForSeconds(0.5f);
        _gameManager.GameState = EGameStateType.Move;
    }
}