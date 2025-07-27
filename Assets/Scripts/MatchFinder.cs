using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

/// <summary>
/// MatchFinder�� ���� �� 3�� ���� ��ġ ���θ� �Ǵ��ϰ�, ��ġ�� ������ ���� �� ����
/// ��ź ���� ��ġ�� ó���ϸ�, ��ü ���� ��ĵ ��ɵ� ����
/// </summary>
public class MatchFinder : MonoBehaviour
{
    List<Fruit> _matchFruits = new List<Fruit>();
    List<Fruit> _fruitsList = new List<Fruit>();
    List<Fruit> _bombs = new List<Fruit>();

    FruitManager _fruitManager;
    BombManager _bombManager;

    public List<Fruit> MatchFruits { get => _matchFruits; }

    void Start()
    {
        _fruitManager = GenericSingleton<FruitManager>.Instance;
        _bombManager = GenericSingleton<BombManager>.Instance;
    }

    /// <summary>
    /// �־��� ���� �迭�� ��ġ ������ �����ϴ��� �˻��ϰ� ��ġ ����Ʈ�� ���
    /// </summary>
    void FindFruitMatch(Fruit[] fruits, ELineBombDirectionType direction)
    {
        ClearList();

        // ���� �迭���� ��ź�� �Ϲ� ���� �и�
        BombCount(fruits);

        // ��ź�� ���Ե� ���
        if (_bombs.Count > 0)
        {
            // ��ź ��ġ �˻� �� ���
            CheckBomb(fruits, direction);
        }
        else if (fruits[0].FruitType == fruits[1].FruitType && fruits[1].FruitType == fruits[2].FruitType)
        {
            // �Ϲ� ���� 3���� ���� Ÿ���� �� ��ġ ���
            FruitMatch(fruits);
        }
        ClearList();
    }

    /// <summary>
    /// ��ź ���� ��ġ���� Ȯ���ϰ� ��ȿ�ϸ� ��ź ���� ���� �� ��ġ ���
    /// </summary>
    void CheckBomb(Fruit[] fruits, ELineBombDirectionType direction)
    {
        if (CheckBombMatch())
        {
            _bombManager.LineBombDirection = direction;
            FruitMatch(fruits);
        }
    }

    /// <summary>
    /// ���� �迭�� ��ġ ����Ʈ�� ���
    /// </summary>
    void FruitMatch(Fruit[] fruits)
    {
        for (int i = 0; i < fruits.Length; i++)
            AddMatchFruits(fruits[i]);
    }

    /// <summary>
    /// ��ġ ����Ʈ�� ������ �ߺ� ���� �߰��ϰ�, IsMatch �÷��� ����
    /// </summary>
    void AddMatchFruits(Fruit fruit)
    {
        if (!_matchFruits.Contains(fruit))
            _matchFruits.Add(fruit);

        if (!fruit.IsMatch)
            fruit.IsMatch = true;
    }

    /// <summary>
    /// ���޹��� ���� �� ��ź ���θ� �������� ��ź�� �Ϲ� ������ �з�
    /// </summary>
    void BombCount(Fruit[] fruits)
    {
        _bombs.Clear();
        _fruitsList.Clear();

        foreach (Fruit fruit in fruits)
        {
            // ��ź�̸� _bombs ����Ʈ�� �߰�
            if (fruit.IsBomb)
                _bombs.Add(fruit);
            else
            {
                // �ƴϸ� �Ϲ� ���� ����Ʈ�� �߰�
                _fruitsList.Add(fruit);
            }
        }
    }

    /// <summary>
    /// ��ź ��ġ�� ��ȿ���� Ȯ��
    /// ��ź ������ ����, Ÿ�� ��ġ ���ο� ���� ��ġ ���� ���θ� �Ǵ�
    /// </summary>
    bool CheckBombMatch()
    {
        switch (_bombs.Count)
        {
            case 3: // ��ź 3�� ��� ���� �������� �˻�
                return _bombs.All(b => b.ColorType == _bombs[0].ColorType);

            case 2: // ��ź 2�� ���� ����, �Ϲ� ���� ���� ����
                return _bombs[0].ColorType == _bombs[1].ColorType &&
                       _bombs[0].ColorType == _fruitsList[0].ColorType;

            case 1: // ��ź 1�� ����� �Ϲ� ���� ���� ����, �Ϲ� ���� Ÿ�Ե� ����
                return _bombs[0].ColorType == _fruitsList[0].ColorType &&
                       _fruitsList[0].FruitType == _fruitsList[1].FruitType;

            default:
                return false;
        }
    }

    /// <summary>
    /// ��ź, �Ϲ� ����Ʈ �ʱ�ȭ
    /// </summary>
    void ClearList()
    {
        _fruitsList.Clear();
        _bombs.Clear();
    }

    /// <summary>
    /// �־��� ���� 3���� ��ġ ������ �����ϴ��� �˻�
    /// </summary>
    public bool CheckMatch(Fruit[] fruits)
    {
        // ��ź�� �Ϲ� ���� �и�
        BombCount(fruits);

        // ��ź ���� ��ġ �˻�
        if (_bombs.Count > 0)      
        {
            if (CheckBombMatch())
            {
                ClearList();
                return true;
            }
        }

        // �Ϲ� ��ġ
        else if (fruits[0].FruitType == fruits[1].FruitType && fruits[1].FruitType == fruits[2].FruitType) 
        {
            ClearList();
            return true;
        }

        ClearList();
        return false;
    }

    /// <summary>
    /// ��ü ���忡�� ��ġ ������ ã�� ��ġ ����Ʈ�� ����ϴ� �ڷ�ƾ ����
    /// </summary>
    public void FindAllMatch()
    {
        StartCoroutine(FindAllMatchRoutine());
    }

    /// <summary>
    /// ���� ��ü�� ��ȸ�ϸ� ����, ���� �������� 3�� ��ġ �˻�
    /// </summary>
    IEnumerator FindAllMatchRoutine()
    {
        Fruit[] fruits = new Fruit[3];
        yield return null;  

        for (int i = 0; i < _fruitManager.Width; i++)
        {
            for (int j = 0; j < _fruitManager.Height; j++)
            {
                fruits[0] = _fruitManager.AllFruits[i, j];
                if (fruits[0] == null)
                    continue;

                // ���� ���� (���Ʒ� ���� �˻�)
                if (i > 0 && i < _fruitManager.Width - 1)
                {
                    fruits[1] = _fruitManager.AllFruits[i - 1, j];
                    fruits[2] = _fruitManager.AllFruits[i + 1, j];

                    if (fruits[1] != null && fruits[2] != null)
                        FindFruitMatch(fruits, ELineBombDirectionType.Column);
                }

                // ���� ���� (�¿� ���� �˻�)
                if (j > 0 && j < _fruitManager.Height - 1)
                {
                    fruits[1] = _fruitManager.AllFruits[i, j + 1];
                    fruits[2] = _fruitManager.AllFruits[i, j - 1];

                    if (fruits[1] != null && fruits[2] != null)
                        FindFruitMatch(fruits, ELineBombDirectionType.Row);
                }
            }
        }
    }
}
