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
        _eventUIManager.EventUI.TryGetValue(_eventType, out _eventUI);
    }

    public override void EventEffect()
    {
        _gameManager.GameState = EGameStateType.Event;
        StartCoroutine(ShuffleFruitRoutine());
    }

    void CreateFruit(int column, int row)
    {
        List<GameObject> fruits = _fruitManager.Fruits;
        Vector2 position = new Vector2(column, row);
        int fruitNumber = 0;
        int iteration = 0;
        do
        {
            fruitNumber = Random.Range(0, fruits.Count);
            iteration++;
        }
        while (_fruitManager.MatchAt(column, row, fruits[fruitNumber]) && iteration <= 100);

        GameObject temp = Instantiate(fruits[fruitNumber], position, Quaternion.identity);
        Fruit fruit = temp.GetComponent<Fruit>();
        fruit.Column = column;
        fruit.Row = row;
        _fruitManager.AllFruits[column, row] = fruit;
    }

    public void ShuffleFruit()
    {
        _endShuffle = false;
        List<Fruit> newFruit = new List<Fruit>();

        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (!_tileManager.BlankTiles[i, j] && _tileManager.ConcreteTiles[i, j] == null && _tileManager.LavaTiles[i, j] == null)
                {
                    if (_fruitManager.AllFruits[i, j] != null)
                        newFruit.Add(_fruitManager.AllFruits[i, j]);
                }
            }
        }
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (!_tileManager.BlankTiles[i, j] && _tileManager.ConcreteTiles[i, j] == null && _tileManager.LavaTiles[i, j] == null)
                {
                    if (newFruit.Count != 0)
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

                    if (newFruit.Count == 0)
                    {
                        CreateFruit(i, j);
                    }
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
        _eventUI.OnEventUI();
        yield return new WaitForSeconds(_eventUIDelay);

        ShuffleFruit();
        while (!_endShuffle)
        {
            yield return new WaitForSeconds(0.1f);
            if (_endShuffle)
                break;
        }

        yield return new WaitForSeconds(_eventDelay);
        _eventUI.OffEventUI();
        yield return new WaitForSeconds(_eventUIDelay);
        _gameManager.GameState = EGameStateType.Move;
    }
}