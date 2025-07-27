using System.Collections.Generic;
using UnityEngine;
using Utils;

/// <summary>
/// BombManager는 폭탄 생성 조건 검사 및 폭탄 생성,  
/// 폭탄과 관련된 타일 파괴 처리, 그리고 폭탄 방향 전환 등을 담당
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
    /// 각종 매니저 인스턴스 초기화
    /// </summary>
    public void Init()
    {
        // 싱글턴 인스턴스 할당
        _factoryManager = GenericSingleton<FactoryManager>.Instance;
        _fruitManager = GenericSingleton<FruitManager>.Instance;
        _tileManager = GenericSingleton<TileManager>.Instance;
        _matchFinder = GenericSingleton<MatchFinder>.Instance;
    }

    /// <summary>
    /// 특정 과일이 폭탄 생성 조건에 해당하는지 체크하고,  
    /// 조건에 맞으면 폭탄 생성 함수를 호출
    /// </summary>
    void CheckCreatableBomb(Fruit fruit)
    {
        // 같은 열과 행에서 매치된 과일 수 계산
        CalculateMatch(out int columnMatch, out int rowMatch, fruit);

        if (_creatableFruits.Count == 0)
        {
            _creatableFruits.Clear();
            return;
        }

        Fruit currentFruit = _fruitManager.CurrentFruit;

        // 현재 과일과 관련 과일이 폭탄 생성 조건에 해당하는지 검사
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

        // 검사 완료 후 리스트 초기화
        _creatableFruits.Clear();
    }


    /// <summary>
    /// 매치된 과일 리스트를 받아 특정 과일 기준  
    /// 같은 열과 행에 매치된 과일 수를 계산
    /// </summary>
    void CalculateMatch(out int columnMatch, out int rowMatch, Fruit fruit)
    {
        List<Fruit> matchFruits = _matchFinder.MatchFruits;

        // 시작점으로 입력 과일 추가
        _creatableFruits.Clear();
        _creatableFruits.Add(fruit);

        columnMatch = 0;
        rowMatch = 0;

        // 매치된 모든 과일에 대해 비교
        foreach (var nextFruit in matchFruits)
        {
            if (nextFruit == fruit)
                continue;   // 자기 자신은 비교 대상 제외

            // 같은 열에서 폭탄 여부와 색상/타입 조건 검사
            if (CalculateColumnMatch(fruit, nextFruit))
            {
                columnMatch++;
                if (!_creatableFruits.Contains(nextFruit))
                    _creatableFruits.Add(nextFruit);
            }

            // 같은 행에서 폭탄 여부와 색상/타입 조건 검사
            if (CalculateRowMatch(fruit, nextFruit))
            {
                rowMatch++;
                if (!_creatableFruits.Contains(nextFruit))
                    _creatableFruits.Add(nextFruit);
            }
        }
    }


    /// <summary>
    /// 두 과일이 같은 열에 있고, 폭탄 여부 및 색상/타입이 일치하는지 확인
    /// </summary>
    bool CalculateColumnMatch(Fruit fruit, Fruit nextFruit)
    {
        // 폭탄이 포함된 경우, 색상과 열이 같은지 확인
        if (fruit.IsBomb || nextFruit.IsBomb)
        {
            if (fruit.Column == nextFruit.Column && fruit.ColorType == nextFruit.ColorType)
                return true;
        }
        // 폭탄이 없으면 타입과 열 비교
        if (fruit.Column == nextFruit.Column && fruit.FruitType == nextFruit.FruitType)
            return true;

        return false;
    }

    /// <summary>
    /// 두 과일이 같은 행에 있고, 폭탄 여부 및 색상/타입이 일치하는지 확인
    /// </summary>
    bool CalculateRowMatch(Fruit fruit, Fruit nextFruit)
    {
        // 폭탄이 포함된 경우, 색상과 행이 같은지 확인
        if (fruit.IsBomb || nextFruit.IsBomb)
        {
            if (fruit.Row == nextFruit.Row && fruit.ColorType == nextFruit.ColorType)
                return true;
        }
        // 폭탄이 없으면 타입과 행 비교
        if (fruit.Row == nextFruit.Row && fruit.FruitType == nextFruit.FruitType)
            return true;

        return false;
    }

    /// <summary>
    /// 계산된 열/행 매치 수에 따라 적절한 폭탄 타입을 선택하여 생성 호출
    /// </summary>
    void SelectCreateBomb(int columnMatch, int rowMatch, Fruit fruit)
    {
        // 5개 이상 매치 시 FruitBomb 생성
        if (columnMatch == 4 || rowMatch == 4)
            CreateBomb(EBombType.FruitBomb, fruit);

        // 2x2 매치 시 SquareBomb 생성
        else if (columnMatch == 2 && rowMatch == 2)
            CreateBomb(EBombType.SquareBomb, fruit);

        // 4개 매치 시 LineBomb 생성
        else if (columnMatch == 3 || rowMatch == 3)
            CreateBomb(EBombType.LineBomb, fruit);
    }

    /// <summary>
    /// 특정 위치에 폭탄을 생성하고 기존 과일을 대체합니다.
    /// </summary>
    void CreateBomb(EBombType bombType, Fruit fruit)
    {
        // 폭탄 생성 위치
        Vector2Int position = new(fruit.Column, fruit.Row);

        // 폭탄 생성
        _factoryManager.ColorType = fruit.ColorType;
        Bomb bomb = _factoryManager.MakeObject<EBombType, Bomb>(bombType, position);

        // 위치 동기화
        bomb.transform.position = fruit.transform.position;

        // 기존 과일 제거 및 배열 업데이트
        _fruitManager.DestroyFruit(fruit);
        _fruitManager.AllFruits[position.x, position.y] = bomb;

        // 매치 리스트 초기화
        _matchFinder.MatchFruits.Clear();
    }


    /// <summary>
    /// 특정 위치에 콘크리트 타일이 있으면 파괴 처리
    /// </summary>
    void CheckConcreteTile(int column, int row)
    {
        if (_tileManager.ConcreteTiles[column, row])
            _tileManager.DestroyTile(_tileManager.ConcreteTiles[column, row]);
    }

    /// <summary>
    /// 특정 위치에 용암 타일이 있으면 파괴 처리 및 추가 생성 중지
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
    /// 현재 매치된 과일들에 대해 폭탄 생성 조건을 모두 검사
    /// </summary>
    public void CheckMakeBomb()
    {
        List<Fruit> matchFruits = _matchFinder.MatchFruits;

        // 매치된 과일 각각에 대해 폭탄 생성 조건 체크
        for (int i = 0; i < matchFruits.Count; i++)
            CheckCreatableBomb(matchFruits[i]);
    }

    /// <summary>
    /// 특정 위치의 콘크리트와 용암 타일 파괴 체크 및 처리
    /// </summary>
    public void CheckTile(int column, int row)
    {
        CheckConcreteTile(column, row);
        CheckLavaTile(column, row);
    }

    /// <summary>
    /// 폭탄의 방향을 행/열 사이에서 전환
    /// </summary>
    public void ChangeLineDirection()
    {
        if (_lineBombDirection == ELineBombDirectionType.Column)
            _lineBombDirection = ELineBombDirectionType.Row;
        else if (_lineBombDirection == ELineBombDirectionType.Row)
            _lineBombDirection = ELineBombDirectionType.Column;
    }
}