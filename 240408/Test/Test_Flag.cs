using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Flag : TestBase
{
    Board board;

    private void Start()
    {
        //board = FindAnyObjectByType<Board>();
        //board.Initialize(8, 8, 10);
    }
}

/// 실습_240408
/// 닫혀있는 셀을 우클릭할 때마다 커버의 모양이 변경되어야 한다.
/// 닫혀있는 셀의 상태 : None, Flag, Question
///     None -> Flag
///     Flag -> Question
///     Question -> None

/// 실습_240408
/// 보드가 초기화될 때, mineCount가 FlagCounter에 설정된다.
/// 보드에 깃발을 설치하면 FlagCounter가 감소한다.
/// 보드에서 깃발 설치를 해제하면 FlagCounter가 증가한다.
/// -------------------------------------------------------------
/// [내 생각]
/// >> GameManager의 int flagCount = mineCount;로 수정하기
/// >> Cell의 CellRightPress() 중
///    (1) CellCoverState.None인 상태에서 IncreaseFlagCount() 실행하기
///    (2) CellCoverState.Flag인 상태에서 DecreaseFlagCount() 실행하기
