using System.Collections.Generic;
using UnityEngine;
using Utils;

/// <summary>
/// BombManager�� ��ź ���� ���� �˻� �� ��ź ����,  
/// ��ź�� ���õ� Ÿ�� �ı� ó��, �׸��� ��ź ���� ��ȯ ���� ���
/// </summary>
public class BombManager : MonoBehaviour
{
    List<Fruit> _creatableFruits = new List<Fruit>();

    FactoryManager _factoryManager;
    FruitManager _fruitManager;
    TileManager _tileManager;
    MatchFinder _matchFinder;
    ELineBombDirectionType _lineBombDirection;

    public ELineBombDirectionType LineBombDirection { get => _lineBombDirection; set => _lineBombDirection = value; }

    /// <summary>
    /// ���� �Ŵ��� �ν��Ͻ� �ʱ�ȭ
    /// </summary>
    public void Init()
    {
        // �̱��� �ν��Ͻ� �Ҵ�
        _factoryManager = GenericSingleton<FactoryManager>.Instance;
        _fruitManager = GenericSingleton<FruitManager>.Instance;
        _tileManager = GenericSingleton<TileManager>.Instance;
        _matchFinder = GenericSingleton<MatchFinder>.Instance;
    }

    /// <summary>
    /// Ư�� ������ ��ź ���� ���ǿ� �ش��ϴ��� üũ�ϰ�,  
    /// ���ǿ� ������ ��ź ���� �Լ��� ȣ��
    /// </summary>
    void CheckCreatableBomb(Fruit fruit)
    {
        // ���� ���� �࿡�� ��ġ�� ���� �� ���
        CalculateMatch(out int columnMatch, out int rowMatch, fruit);

        if (_creatableFruits.Count == 0)
        {
            _creatableFruits.Clear();
            return;
        }

        Fruit currentFruit = _fruitManager.CurrentFruit;

        // ���� ���ϰ� ���� ������ ��ź ���� ���ǿ� �ش��ϴ��� �˻�
        foreach (var creatableFruit in _creatableFruits)
        {
            if (creatableFruit == currentFruit)
            {
                SelectCreateBomb(columnMatch, rowMatch, currentFruit);
            }
            else if (creatableFruit == currentFruit.OtherFruit)
            {
                SelectCreateBomb(columnMatch, rowMatch, currentFruit.OtherFruit);
            }
        }

        // �˻� �Ϸ� �� ����Ʈ �ʱ�ȭ
        _creatableFruits.Clear();
    }


    /// <summary>
    /// ��ġ�� ���� ����Ʈ�� �޾� Ư�� ���� ����  
    /// ���� ���� �࿡ ��ġ�� ���� ���� ���
    /// </summary>
    void CalculateMatch(out int columnMatch, out int rowMatch, Fruit fruit)
    {
        List<Fruit> matchFruits = _matchFinder.MatchFruits;

        // ���������� �Է� ���� �߰�
        _creatableFruits.Clear();
        _creatableFruits.Add(fruit);

        columnMatch = 0;
        rowMatch = 0;

        // ��ġ�� ��� ���Ͽ� ���� ��
        foreach (var nextFruit in matchFruits)
        {
            if (nextFruit == fruit)
                continue;   // �ڱ� �ڽ��� �� ��� ����

            // ���� ������ ��ź ���ο� ����/Ÿ�� ���� �˻�
            if (CalculateColumnMatch(fruit, nextFruit))
            {
                columnMatch++;
                if (!_creatableFruits.Contains(nextFruit))
                    _creatableFruits.Add(nextFruit);
            }

            // ���� �࿡�� ��ź ���ο� ����/Ÿ�� ���� �˻�
            if (CalculateRowMatch(fruit, nextFruit))
            {
                rowMatch++;
                if (!_creatableFruits.Contains(nextFruit))
                    _creatableFruits.Add(nextFruit);
            }
        }
    }


    /// <summary>
    /// �� ������ ���� ���� �ְ�, ��ź ���� �� ����/Ÿ���� ��ġ�ϴ��� Ȯ��
    /// </summary>
    bool CalculateColumnMatch(Fruit fruit, Fruit nextFruit)
    {
        // ��ź�� ���Ե� ���, ����� ���� ������ Ȯ��
        if (fruit.IsBomb || nextFruit.IsBomb)
        {
            if (fruit.Column == nextFruit.Column && fruit.ColorType == nextFruit.ColorType)
                return true;
        }
        // ��ź�� ������ Ÿ�԰� �� ��
        if (fruit.Column == nextFruit.Column && fruit.FruitType == nextFruit.FruitType)
            return true;

        return false;
    }

    /// <summary>
    /// �� ������ ���� �࿡ �ְ�, ��ź ���� �� ����/Ÿ���� ��ġ�ϴ��� Ȯ��
    /// </summary>
    bool CalculateRowMatch(Fruit fruit, Fruit nextFruit)
    {
        // ��ź�� ���Ե� ���, ����� ���� ������ Ȯ��
        if (fruit.IsBomb || nextFruit.IsBomb)
        {
            if (fruit.Row == nextFruit.Row && fruit.ColorType == nextFruit.ColorType)
                return true;
        }
        // ��ź�� ������ Ÿ�԰� �� ��
        if (fruit.Row == nextFruit.Row && fruit.FruitType == nextFruit.FruitType)
            return true;

        return false;
    }

    /// <summary>
    /// ���� ��/�� ��ġ ���� ���� ������ ��ź Ÿ���� �����Ͽ� ���� ȣ��
    /// </summary>
    void SelectCreateBomb(int columnMatch, int rowMatch, Fruit fruit)
    {
        // 5�� �̻� ��ġ �� FruitBomb ����
        if (columnMatch == 4 || rowMatch == 4)
            CreateBomb(EBombType.FruitBomb, fruit);

        // 2x2 ��ġ �� SquareBomb ����
        else if (columnMatch == 2 && rowMatch == 2)
            CreateBomb(EBombType.SquareBomb, fruit);

        // 4�� ��ġ �� LineBomb ����
        else if (columnMatch == 3 || rowMatch == 3)
            CreateBomb(EBombType.LineBomb, fruit);
    }

    /// <summary>
    /// Ư�� ��ġ�� ��ź�� �����ϰ� ���� ������ ��ü�մϴ�.
    /// </summary>
    void CreateBomb(EBombType bombType, Fruit fruit)
    {
        // ��ź ���� ��ġ
        Vector2Int position = new(fruit.Column, fruit.Row);

        // ��ź ����
        _factoryManager.ColorType = fruit.ColorType;
        Bomb bomb = _factoryManager.MakeObject<EBombType, Bomb>(bombType, position);

        // ��ġ ����ȭ
        bomb.transform.position = fruit.transform.position;

        // ���� ���� ���� �� �迭 ������Ʈ
        _fruitManager.DestroyFruit(fruit);
        _fruitManager.AllFruits[position.x, position.y] = bomb;

        // ��ġ ����Ʈ �ʱ�ȭ
        _matchFinder.MatchFruits.Clear();
    }


    /// <summary>
    /// Ư�� ��ġ�� ��ũ��Ʈ Ÿ���� ������ �ı� ó��
    /// </summary>
    void CheckConcreteTile(int column, int row)
    {
        if (_tileManager.ConcreteTiles[column, row])
            _tileManager.DestroyTile(_tileManager.ConcreteTiles[column, row]);
    }

    /// <summary>
    /// Ư�� ��ġ�� ��� Ÿ���� ������ �ı� ó�� �� �߰� ���� ����
    /// </summary>
    void CheckLavaTile(int column, int row)
    {
        if (_tileManager.LavaTiles[column, row])
        {
            _tileManager.DestroyTile(_tileManager.LavaTiles[column, row]);
            _tileManager.CreateMoreLavaTile = false;
        }
    }

    /// <summary>
    /// ���� ��ġ�� ���ϵ鿡 ���� ��ź ���� ������ ��� �˻�
    /// </summary>
    public void CheckMakeBomb()
    {
        List<Fruit> matchFruits = _matchFinder.MatchFruits;

        // ��ġ�� ���� ������ ���� ��ź ���� ���� üũ
        for (int i = 0; i < matchFruits.Count; i++)
            CheckCreatableBomb(matchFruits[i]);
    }

    /// <summary>
    /// Ư�� ��ġ�� ��ũ��Ʈ�� ��� Ÿ�� �ı� üũ �� ó��
    /// </summary>
    public void CheckTile(int column, int row)
    {
        CheckConcreteTile(column, row);
        CheckLavaTile(column, row);
    }

    /// <summary>
    /// ��ź�� ������ ��/�� ���̿��� ��ȯ
    /// </summary>
    public void ChangeLineDirection()
    {
        if (_lineBombDirection == ELineBombDirectionType.Column)
            _lineBombDirection = ELineBombDirectionType.Row;
        else if (_lineBombDirection == ELineBombDirectionType.Row)
            _lineBombDirection = ELineBombDirectionType.Column;
    }
}