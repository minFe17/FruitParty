using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuffle : Event
{
    protected override void Start()
    {
        base.Start();
        _eventType = EEventType.Shuffle;
    }

    public override void EventEffect()
    {
        _gameManager.GameState = EGameStateType.Event;
        StartCoroutine(ShuffleFruitRoutine());
    }

    void ShuffleFruit()
    {
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
    }

    IEnumerator ShuffleFruitRoutine()
    {
        //�̹��� ���̱�
        yield return new WaitForSeconds(0.5f);
        ShuffleFruit();
        if (_fruitManager.IsDeadlocked())
            ShuffleFruit();
        else
        {
            //�̹��� �����
            yield return new WaitForSeconds(0.5f);
            _gameManager.GameState = EGameStateType.Move;
        }
    }
}