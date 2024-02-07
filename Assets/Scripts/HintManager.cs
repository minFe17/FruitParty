using System.Collections.Generic;
using UnityEngine;
using Utils;

public class HintManager : MonoBehaviour
{
    // ╫л╠шео
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
        _hintDelay = 10f;
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
        Fruit movableFruit = PickOneRandomFruit();
        if (movableFruit != null)
        {
            Vector2Int position = new Vector2Int(movableFruit.Column, movableFruit.Row);
            _factoryManager.MakeObject<EEffectType, GameObject>(EEffectType.Hint, position);
        }
    }

    Fruit PickOneRandomFruit()
    {
        List<Fruit> movableFruits;
        FindMovableFruit(out movableFruits);
        if (movableFruits.Count > 0)
        {
            int fruitIndex = Random.Range(0, movableFruits.Count);
            return movableFruits[fruitIndex];
        }
        return null;
    }

    void FindMovableFruit(out List<Fruit> movableFruits)
    {
        movableFruits = new List<Fruit>();

        for (int i = 0; i < _fruitManager.Width; i++)
        {
            for (int j = 0; j < _fruitManager.Height; j++)
            {
                if (_fruitManager.AllFruits[i, j] != null)
                {
                    if (i < _fruitManager.Width - 1)
                    {
                        if (_fruitManager.SwitchAndCheck(i, j, Vector2Int.right))
                            movableFruits.Add(_fruitManager.AllFruits[i, j]);
                    }
                    if (j < _fruitManager.Height - 1)
                    {
                        if (_fruitManager.SwitchAndCheck(i, j, Vector2Int.up))
                            movableFruits.Add(_fruitManager.AllFruits[i, j]);
                    }
                }
            }
        }
    }

    public void DestroyHint()
    {
        if (_currentHint != null)
        {
            _objectPoolManager.Pull(EEffectType.Hint, _currentHint);
            _currentHint = null;
            _hintCoolTime = _hintDelay;
        }
    }
}