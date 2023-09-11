using System.Collections.Generic;
using UnityEngine;
using Utils;

public class HintManager : MonoBehaviour
{
    // ╫л╠шео
    FruitManager _fruitManager;
    GameObject _hintEffect;
    GameObject _currentHint;

    float _hintDelay;
    float _hintDelayTime;

    void Update()
    {
        ShowHintTimeCheck();
    }

    public void Init()
    {
        _fruitManager = GenericSingleton<FruitManager>.Instance;
        _hintEffect = Resources.Load("Prefabs/Effect/HintEffect") as GameObject;
        _hintDelay = 10f;
        _hintDelayTime = _hintDelay;
    }

    void ShowHintTimeCheck()
    {
        _hintDelayTime -= Time.deltaTime;
        if (_hintDelayTime <= 0 && _currentHint == null)
        {
            ShowHint();
            _hintDelayTime = _hintDelay;
        }
    }

    void ShowHint()
    {
        Fruit movableFruit = PickOneRandomFruit();
        if(movableFruit != null)
        {
            _currentHint = Instantiate(_hintEffect, movableFruit.transform.position, Quaternion.identity);
        }
    }

    Fruit PickOneRandomFruit()
    {
        List<Fruit> movableFruits = new List<Fruit>();
        movableFruits = FindMovableFruit();
        if (movableFruits.Count > 0)
        {
            int fruitIndex = Random.Range(0, movableFruits.Count);
            return movableFruits[fruitIndex];
        }
        return null;
    }

    List<Fruit> FindMovableFruit()
    {
        List<Fruit> movableFruits = new List<Fruit>();

        for (int i = 0; i < _fruitManager.Width; i++)
        {
            for (int j = 0; j < _fruitManager.Height; j++)
            {
                if (_fruitManager.AllFruits[i, j] != null)
                {
                    if (i < _fruitManager.Width - 1)
                    {
                        if (_fruitManager.SwitchAndCheck(i, j, Vector2.right))
                            movableFruits.Add(_fruitManager.AllFruits[i, j]);
                    }
                    if (j < _fruitManager.Height - 1)
                    {
                        if (_fruitManager.SwitchAndCheck(i, j, Vector2.up))
                            movableFruits.Add(_fruitManager.AllFruits[i, j]);
                    }
                }
            }
        }
        return movableFruits;
    }

    public void DestroyHint()
    {
        if(_currentHint != null)
        {
            Destroy(_currentHint);
            _currentHint = null;
            _hintDelayTime = _hintDelay;
        }
    }
}
