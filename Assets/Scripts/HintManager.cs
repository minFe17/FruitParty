using System.Collections.Generic;
using UnityEngine;
using Utils;

public class HintManager : MonoBehaviour
{
    // 싱글턴
    List<Fruit> _movableFruits = new List<Fruit>();

    FactoryManager _factoryManager;
    ObjectPoolManager _objectPoolManager;
    FruitManager _fruitManager;
    GameObject _currentHint;

    float _hintDelay;
    float _hintCoolTime;

    void Update()
    {
        ShowHintTimeCheck();
    }

    public void Init()
    {
        _factoryManager = GenericSingleton<FactoryManager>.Instance;
        _objectPoolManager = GenericSingleton<ObjectPoolManager>.Instance;
        _fruitManager = GenericSingleton<FruitManager>.Instance;
        _hintDelay = 5f;
        _hintCoolTime = _hintDelay;
    }

    void ShowHintTimeCheck()
    {
        GameManager gameManager = GenericSingleton<GameManager>.Instance;
        if (gameManager.GameState == EGameStateType.Move)
        {
            _hintCoolTime -= Time.deltaTime;
            if (_hintCoolTime <= 0 && _currentHint == null)
            {
                ShowHint();
                _hintCoolTime = _hintDelay;
            }
        }
        else
            _hintCoolTime = _hintDelay;
    }

    void ShowHint()
    {
        _movableFruits.Clear();
        Fruit movableFruit = PickOneRandomFruit();
        if (movableFruit != null)
        {
            Vector2Int position = new Vector2Int(movableFruit.Column, movableFruit.Row);
            _currentHint = _factoryManager.MakeObject<EEffectType, GameObject>(EEffectType.Hint, position);
        }
    }

    Fruit PickOneRandomFruit()
    {
        FindMovableFruit();
        if (_movableFruits.Count > 0)
        {
            int fruitIndex = Random.Range(0, _movableFruits.Count);
            return _movableFruits[fruitIndex];
        }
        _movableFruits.Clear();
        return null;
    }

    void FindMovableFruit()
    {
        _movableFruits.Clear();

        for (int i = 0; i < _fruitManager.Width; i++)
        {
            for (int j = 0; j < _fruitManager.Height; j++)
            {
                Fruit fruit = _fruitManager.AllFruits[i, j];
                if (fruit == null) continue;

                // 오른쪽 검사
                if (i < _fruitManager.Width - 1)
                {
                    Fruit right = _fruitManager.AllFruits[i + 1, j];
                    if (right != null)
                    {
                        if (_fruitManager.SwitchAndCheck(i, j, Vector2Int.right))
                        {
                            _movableFruits.Add(fruit);
                            continue; // 이미 오른쪽 가능하면 추가됨
                        }
                    }
                }

                // 위쪽 검사
                if (j < _fruitManager.Height - 1)
                {
                    Fruit up = _fruitManager.AllFruits[i, j + 1];
                    if (up != null)
                    {
                        if (_fruitManager.SwitchAndCheck(i, j, Vector2Int.up))
                        {
                            _movableFruits.Add(fruit);
                        }
                    }
                }
            }
        }
    }


    public void DestroyHint()
    {
        if (_currentHint != null)
        {
            _objectPoolManager.Push(EEffectType.Hint, _currentHint);
            _currentHint = null;
            _hintCoolTime = _hintDelay;
        }
    }
}