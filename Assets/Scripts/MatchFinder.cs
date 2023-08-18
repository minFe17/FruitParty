using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class MatchFinder : MonoBehaviour
{
    //╫л╠шео
    List<Fruit> _matchFruits = new List<Fruit>();
    FruitManager _fruitManager;

    public List<Fruit> MatchFruits { get => _matchFruits; }

    void Start()
    {
        _fruitManager = GenericSingleton<FruitManager>.Instance;
    }

    public void FindAllMatch()
    {
        StartCoroutine(FindAllMatchRoutine());
    }

    IEnumerator FindAllMatchRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < _fruitManager.Width; i++)
        {
            for (int j = 0; j < _fruitManager.Height; j++)
            {
                Fruit currentFruit = _fruitManager.AllFruits[i, j];
                if (currentFruit != null)
                {
                    if (i > 0 && i < _fruitManager.Width - 1)
                    {
                        Fruit leftFruit = _fruitManager.AllFruits[i - 1, j];
                        Fruit rightFruit = _fruitManager.AllFruits[i + 1, j];
                        if (leftFruit != null && rightFruit != null)
                        {
                            if (leftFruit.FruitType == currentFruit.FruitType && rightFruit.FruitType == currentFruit.FruitType)
                            {
                                if (!_matchFruits.Contains(currentFruit))
                                    _matchFruits.Add(currentFruit);
                                if (!_matchFruits.Contains(leftFruit))
                                    _matchFruits.Add(leftFruit);
                                if (!_matchFruits.Contains(rightFruit))
                                    _matchFruits.Add(rightFruit);

                                currentFruit.IsMatch = true;
                                leftFruit.IsMatch = true;
                                rightFruit.IsMatch = true;
                            }
                        }
                    }

                    if (j > 0 && j < _fruitManager.Height - 1)
                    {
                        Fruit upFruit = _fruitManager.AllFruits[i, j + 1];
                        Fruit downFruit = _fruitManager.AllFruits[i, j - 1];
                        if (upFruit != null && downFruit != null)
                        {
                            if (upFruit.FruitType == currentFruit.FruitType && downFruit.FruitType == currentFruit.FruitType)
                            {
                                if (!_matchFruits.Contains(currentFruit))
                                    _matchFruits.Add(currentFruit);
                                if (!_matchFruits.Contains(upFruit))
                                    _matchFruits.Add(upFruit);
                                if (!_matchFruits.Contains(downFruit))
                                    _matchFruits.Add(downFruit);

                                currentFruit.IsMatch = true;
                                upFruit.IsMatch = true;
                                downFruit.IsMatch = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
