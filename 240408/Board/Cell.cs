using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    /// <summary>
    /// 이 셀의 ID (위치 계산에도 사용될 수 있음)
    /// </summary>
    int? id = null;
    public int ID
    {
        get => id.GetValueOrDefault(); // 0일 경우, 맞을 수도 있고 아닐수도 있다.
        set
        {
            if (id == null) // 이 프로퍼티는 한 번만 설정 가능하다.
            {
                id = value;
            }
        }
    }

    /// <summary>
    /// 겉면의 스프라이트 랜더러 (Close, Question, Flag)
    /// </summary>
    SpriteRenderer cover;

    /// <summary>
    /// 안쪽의 스프라이트 랜더러 (지뢰, 주변 지뢰 개수)
    /// </summary>
    SpriteRenderer inside;

    /// <summary>
    /// 셀에 지뢰가 있는지 여부
    /// </summary>
    bool hasMine = false;

    /// <summary>
    /// 지뢰 설치 여부를 확인하기 위한 프로퍼티
    /// </summary>
    public bool HasMine => hasMine;

    /// <summary>
    /// 이 셀이 관리되는 보드
    /// </summary>
    Board parentBoard = null;
    public Board Board
    {
        get => parentBoard;
        set
        {
            if (parentBoard == null) // 한 번만 설정 가능
            {
                parentBoard = value;
            }
        }
    }

    /// <summary>
    /// 자기 주변 셀의 목록
    /// </summary>
    List<Cell> neighbors;

    /// <summary>
    /// 자기 주변의 지뢰 개수
    /// </summary>
    int aroundMineCount = 0;

    /// <summary>
    /// 셀이 열렸는지 여부
    /// </summary>
    bool isOpen = false;

    /// <summary>
    /// 셀의 커버 표시 상태용 (닫혔을 때의 상태)
    /// </summary>
    enum CellCoverState
    {
        None = 0,   // 아무것도 표시되지 않은 상태
        Flag,       // 깃발이 표시된 상태
        Question    // 물음표가 표시된 상태
    }

    /// <summary>
    /// 셀의 커버 상태
    /// </summary>
    CellCoverState coverState = CellCoverState.None;

    /// <summary>
    /// 셀의 커버 상태를 설정하고 확인하기 위한 프로퍼티
    /// </summary>
    CellCoverState CoverState
    {
        get => coverState;
        set
        {
            coverState = value;
            switch (coverState)
            {
                case CellCoverState.None:
                    cover.sprite = Board[CloseCellType.Close];
                    break;
                case CellCoverState.Flag:
                    cover.sprite = Board[CloseCellType.Flag];
                    onFlagUse?.Invoke();
                    break;
                case CellCoverState.Question:
                    cover.sprite = Board[CloseCellType.Question];
                    onFlagReturn?.Invoke();
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// 깃발 설치가 완료되었음을 알리는 델리게이트
    /// </summary>
    public Action onFlagUse;

    /// <summary>
    /// 깃발 설치가 취소되었음을 알리는 델리게이트
    /// </summary>
    public Action onFlagReturn;

    /// <summary>
    /// 깃발 설치 여부를 알려주는 프로퍼티
    /// </summary>
    public bool IsFlaged => CoverState == CellCoverState.Flag;

    private void Awake()
    {
        Transform child = transform.GetChild(0);
        cover = child.GetComponent<SpriteRenderer>();
        child = transform.GetChild(1);
        inside = child.GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 셀 생성 초기화 함수 (처음에 단 한 번만 실행되면 됨)
    /// </summary>
    public void Initialize()
    {
        neighbors = Board.GetNeighbors(ID); // 이웃 셀 저장해놓기
    }

    /// <summary>
    /// 이 셀의 데이터를 초기화하는 함수
    /// </summary>
    public void ResetData()
    {
        hasMine = false;
        aroundMineCount = 0;
        isOpen = false;

        cover.sprite = Board[CloseCellType.Close];
        inside.sprite = Board[OpenCellType.Empty];
        cover.gameObject.SetActive(true);
    }

    /// <summary>
    /// 이 셀에 지뢰를 설치하는 함수
    /// </summary>
    public void SetMine()
    {
        hasMine = true;                             // 지뢰 설치했다고 표시
        inside.sprite = Board[OpenCellType.Mine];   // 이미지 변경

        foreach (Cell cell in neighbors)
        {
            cell.IncreaseAroundMineCount(); // 주변 셀의 주변 지뢰 개수 증가
        }
    }

    /// <summary>
    /// 주변 지뢰 개수 증가용 함수
    /// </summary>
    void IncreaseAroundMineCount()
    {
        if (!hasMine) // 지뢰가 아닐 때만 개수 증가
        {
            aroundMineCount++;                                      // 주변 지뢰 개수 증가
            inside.sprite = Board[(OpenCellType)aroundMineCount];   // 주변 지뢰 개수에 맞게 이미지 변경
        }
    }

    /// <summary>
    /// 셀이 우클릭되면 실행되는 함수
    /// </summary>
    public void RightPress()
    {
        switch (CoverState)
        {
            case CellCoverState.None:
                CoverState = CellCoverState.Flag;
                break;
            case CellCoverState.Flag:
                CoverState = CellCoverState.Question;
                break;
            case CellCoverState.Question:
                CoverState = CellCoverState.None;
                break;
        }
    }

    public void LeftPress()
    {
        switch (CoverState)
        {
            case CellCoverState.None:
                cover.sprite = Board[CloseCellType.ClosePress];
                break;
            case CellCoverState.Question:
                cover.sprite = Board[CloseCellType.QuestionPress];
                break;
            //case CellCoverState.Flag:
            default:
                // 하는 일 없음
                break;
        }
    }

    public void LeftRelease()
    {
        //RestoreCover();
        Open();
    }

    void Open()
    {
        if (!isOpen && !IsFlaged)
        {
            isOpen = true;
            cover.gameObject.SetActive(false);

            if (aroundMineCount <= 0)
            {
                foreach (var cell in neighbors)
                {
                    cell.Open();
                }
            }
            else if (hasMine)
            {
                Debug.Log("게임 오버");
            }
        }
    }

    /// <summary>
    /// 원래 커버 이미지로 변경하는 함수
    /// </summary>
    public void RestoreCover()
    {
        switch (CoverState)
        {
            case CellCoverState.None:
                cover.sprite = Board[CloseCellType.Close];
                break;
            case CellCoverState.Question:
                cover.sprite = Board[CloseCellType.Question];
                break;
            //case CellCoverState.Flag:
            default:
                // 하는 일 없음
                break;
        }
    }

#if UNITY_EDITOR
    public void Test_OpenCover()
    {
        cover.gameObject.SetActive(false);
    }
#endif
}

// 지뢰 배치 여부에 따라 Inside 이미지 변경
// 열기 & 닫기
// 보드에 들어온 입력에 따른 Cover 이미지 변경

/// 실습_240405_생각해보기
///// 내가 생각해본 구현 목록
// [변수]
// ...
// ------------------------------------------------------------
// [함수]
// 마우스 클릭 여부에 따른 셀 열기 함수 & 셀 닫기 함수
// Inside에 있는 지뢰 위치에 따라 숫자와 빈칸 이미지를 배치하는 함수
// 보드에 들어온 입력에 따른 Cover 이미지 변경 함수

/// 코드 구현 전 정리_240408
/// 눌렀다 떼기
/// 눌렀을 때
///     - None : Cell_ClosePress가 보여야 한다.
///     - Flag : 변화가 없다.
///     - Question : Cell_Close_QuestionPress가 보여야 한다.
