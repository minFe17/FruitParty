using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuffle : Event, IEvent
{
    List<Fruit> _newFruit = new List<Fruit>();

    bool _endShuffle;
    public bool EndShuffle { get => _endShuffle; }

    protected override void Start()
    {
        base.Start();
        _eventType = EEventType.Shuffle;
        _eventManager.Shuffle = this;
        _eventUIManager.EventUI.TryGetValue(_eventType, out _eventUI);
    }

    void IEvent.EventEffect()
    {
        if (IsDeadlocked())
        {
            _gameManager.ChangeGameState(EGameStateType.Event);
            StartCoroutine(ShuffleFruitRoutine());
        }
    }

    void NewPositionTarget()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (!_tileManager.BlankTiles[i, j] && _tileManager.ConcreteTiles[i, j] == null && _tileManager.LavaTiles[i, j] == null)
                {
                    if (_fruitManager.AllFruits[i, j] != null)
                        _newFruit.Add(_fruitManager.AllFruits[i, j]);
                }
            }
        }
    }

    void FruitNewPosition()
    {
        NewPositionTarget();
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (!_tileManager.BlankTiles[i, j] && _tileManager.ConcreteTiles[i, j] == null && _tileManager.LavaTiles[i, j] == null)
                {
                    if (_newFruit.Count != 0)
                    {
                        int fruitIndex = Random.Range(0, _newFruit.Count);
                        int iterations = 0;
                        while (_fruitManager.MatchAt(i, j, _newFruit[fruitIndex].FruitType) && iterations <= 100)
                        {
                            fruitIndex = Random.Range(0, _newFruit.Count);
                            iterations++;
                        }

                        Fruit fruit = _newFruit[fruitIndex];
                        fruit.Column = i;
                        fruit.Row = j;
                        _fruitManager.AllFruits[i, j] = _newFruit[fruitIndex];
                        _newFruit.Remove(fruit);
                    }
                    if (_newFruit.Count == 0)
                        CreateFruit(i, j);
                }
            }
        }
        _newFruit.Clear();
    }

    void CreateFruit(int column, int row)
    {
        Vector2Int position = new Vector2Int(column, row);
        _fruitManager.CreateFruit(position);
    }

    bool IsDeadlocked()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (_fruitManager.AllFruits[i, j] != null)
                {
                    if (i < _width - 1)
                    {
                        if (!_fruitManager.SwitchAndCheck(i, j, Vector2Int.right))
                        {
                            return false;
                        }
                    }
                    if (j < _height - 1)
                    {
                        if (!_fruitManager.SwitchAndCheck(i, j, Vector2Int.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    public void ShuffleFruit()
    {
        _endShuffle = false;

        FruitNewPosition();
        if (IsDeadlocked())
            ShuffleFruit();
        else
            _endShuffle = true;
    }

    IEnumerator ShuffleFruitRoutine()
    {
        _eventUI.OnEventUI();
        yield return new WaitForSeconds(_eventUIDelay + 0.5f);

        ShuffleFruit();
        while (!_endShuffle)
        {
            yield return new WaitForSeconds(0.1f);
            if (_endShuffle)
                break;
        }
        _fruitManager.CheckMatchFruit();
        yield return new WaitForSeconds(_eventDelay);

        _eventUI.OffEventUI();
        yield return new WaitForSeconds(_eventUIDelay);
        _gameManager.ChangeGameState(EGameStateType.Move);
    }
}