using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

/// <summary>
/// MatchFinder는 보드 내 3개 과일 매치 여부를 판단하고, 매치된 과일을 추적 및 관리
/// 폭탄 포함 매치도 처리하며, 전체 보드 스캔 기능도 제공
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
    /// 주어진 과일 배열이 매치 조건을 만족하는지 검사하고 매치 리스트에 등록
    /// </summary>
    void FindFruitMatch(Fruit[] fruits, ELineBombDirectionType direction)
    {
        ClearList();

        // 과일 배열에서 폭탄과 일반 과일 분리
        BombCount(fruits);

        // 폭탄이 포함된 경우
        if (_bombs.Count > 0)
        {
            // 폭탄 매치 검사 및 등록
            CheckBomb(fruits, direction);
        }
        else if (fruits[0].FruitType == fruits[1].FruitType && fruits[1].FruitType == fruits[2].FruitType)
        {
            // 일반 과일 3개가 동일 타입일 때 매치 등록
            FruitMatch(fruits);
        }
        ClearList();
    }

    /// <summary>
    /// 폭탄 포함 매치인지 확인하고 유효하면 폭탄 방향 설정 후 매치 등록
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
    /// 과일 배열을 매치 리스트에 등록
    /// </summary>
    void FruitMatch(Fruit[] fruits)
    {
        for (int i = 0; i < fruits.Length; i++)
            AddMatchFruits(fruits[i]);
    }

    /// <summary>
    /// 매치 리스트에 과일을 중복 없이 추가하고, IsMatch 플래그 설정
    /// </summary>
    void AddMatchFruits(Fruit fruit)
    {
        if (!_matchFruits.Contains(fruit))
            _matchFruits.Add(fruit);

        if (!fruit.IsMatch)
            fruit.IsMatch = true;
    }

    /// <summary>
    /// 전달받은 과일 중 폭탄 여부를 기준으로 폭탄과 일반 과일을 분류
    /// </summary>
    void BombCount(Fruit[] fruits)
    {
        _bombs.Clear();
        _fruitsList.Clear();

        foreach (Fruit fruit in fruits)
        {
            // 폭탄이면 _bombs 리스트에 추가
            if (fruit.IsBomb)
                _bombs.Add(fruit);
            else
            {
                // 아니면 일반 과일 리스트에 추가
                _fruitsList.Add(fruit);
            }
        }
    }

    /// <summary>
    /// 폭탄 매치가 유효한지 확인
    /// 폭탄 개수와 색상, 타입 일치 여부에 따라 매치 성립 여부를 판단
    /// </summary>
    bool CheckBombMatch()
    {
        switch (_bombs.Count)
        {
            case 3: // 폭탄 3개 모두 같은 색상인지 검사
                return _bombs.All(b => b.ColorType == _bombs[0].ColorType);

            case 2: // 폭탄 2개 색상 같고, 일반 과일 색상도 같음
                return _bombs[0].ColorType == _bombs[1].ColorType &&
                       _bombs[0].ColorType == _fruitsList[0].ColorType;

            case 1: // 폭탄 1개 색상과 일반 과일 색상 같고, 일반 과일 타입도 같음
                return _bombs[0].ColorType == _fruitsList[0].ColorType &&
                       _fruitsList[0].FruitType == _fruitsList[1].FruitType;

            default:
                return false;
        }
    }

    /// <summary>
    /// 폭탄, 일반 리스트 초기화
    /// </summary>
    void ClearList()
    {
        _fruitsList.Clear();
        _bombs.Clear();
    }

    /// <summary>
    /// 주어진 과일 3개가 매치 조건을 만족하는지 검사
    /// </summary>
    public bool CheckMatch(Fruit[] fruits)
    {
        // 폭탄과 일반 과일 분리
        BombCount(fruits);

        // 폭탄 포함 매치 검사
        if (_bombs.Count > 0)      
        {
            if (CheckBombMatch())
            {
                ClearList();
                return true;
            }
        }

        // 일반 매치
        else if (fruits[0].FruitType == fruits[1].FruitType && fruits[1].FruitType == fruits[2].FruitType) 
        {
            ClearList();
            return true;
        }

        ClearList();
        return false;
    }

    /// <summary>
    /// 전체 보드에서 매치 과일을 찾아 매치 리스트에 등록하는 코루틴 시작
    /// </summary>
    public void FindAllMatch()
    {
        StartCoroutine(FindAllMatchRoutine());
    }

    /// <summary>
    /// 보드 전체를 순회하며 가로, 세로 방향으로 3개 매치 검사
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

                // 세로 방향 (위아래 과일 검사)
                if (i > 0 && i < _fruitManager.Width - 1)
                {
                    fruits[1] = _fruitManager.AllFruits[i - 1, j];
                    fruits[2] = _fruitManager.AllFruits[i + 1, j];

                    if (fruits[1] != null && fruits[2] != null)
                        FindFruitMatch(fruits, ELineBombDirectionType.Column);
                }

                // 가로 방향 (좌우 과일 검사)
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
