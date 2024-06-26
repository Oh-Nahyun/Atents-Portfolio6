using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    // 상태 관련 사항 -------------------------------------------------------------

    /// <summary>
    /// 게임 상태 종류
    /// </summary>
    public enum GameState
    {
        Ready,      // 게임 시작 전
        Play,       // 첫번째 셀이 열리거나 깃발이 설치된 이후
        GameClear,   // 모든 지뢰를 찾았을 때
        GameOver    // 지뢰가 있는 셀을 열었을 때
    }

    /// <summary>
    /// 현재 게임 상태
    /// </summary>
    GameState state = GameState.Ready;

    /// <summary>
    /// 상태 변경 및 확인용 프로퍼티
    /// </summary>
    GameState State
    {
        get => state;
        set
        {
            if (state != value)
            {
                state = value;
                switch (state)
                {
                    case GameState.Ready:
                        onGameReady?.Invoke();      // 델리게이트 실행
                        break;
                    case GameState.Play:
                        onGamePlay?.Invoke();
                        break;
                    case GameState.GameClear:
                        onGameGameClear?.Invoke();
                        break;
                    case GameState.GameOver:
                        onGameGameOver?.Invoke();
                        break;
                }
            }
        }
    }

    // 상태 변경 알림용 델리게이트
    public Action onGameReady;
    public Action onGamePlay;
    public Action onGameGameClear;
    public Action onGameGameOver;

    // 보드 생성 관련 사항 ---------------------------------------------------------

    /// <summary>
    /// 보드 생성 시 지뢰 개수 
    /// </summary>
    public int mineCount = 10;

    /// <summary>
    /// 보드 생성 시 가로 길이
    /// </summary>
    public int boardWidth = 8;

    /// <summary>
    /// 보드 생성 시 세로 길이
    /// </summary>
    public int boardHeight = 8;

    /// <summary>
    /// 게임 보드
    /// </summary>
    Board board;

    /// <summary>
    /// 게임 보드 확인용 프로퍼티
    /// </summary>
    public Board Board => board;


    // 깃발 관련 사항 -------------------------------------------------------------

    /// <summary>
    /// 깃발 개수
    /// </summary>
    int flagCount = 0;

    /// <summary>
    /// 깃발 개수 확인 및 설정용 프로퍼티
    /// </summary>
    public int FlagCount
    {
        get => flagCount;
        private set                 // 쓰기는 this에서만 가능
        {
            if (flagCount != value) // 변경되었을 때만 실행
            {
                flagCount = value;
                onFlagCountChange?.Invoke(flagCount); // 델리게이트로 변경된 값 알리기
            }
        }
    }

    /// <summary>
    /// 깃발 개수가 변경될 때마다 실행되는 델리게이트
    /// </summary>
    public Action<int> onFlagCountChange;

    /// <summary>
    /// 깃발 개수를 1 증가시키는 함수
    /// </summary>
    public void IncreaseFlagCount()
    {
        FlagCount++;
    }

    /// <summary>
    /// 깃발 개수를 1 감소시키는 함수
    /// </summary>
    public void DecreaseFlagCount()
    {
        FlagCount--;
    }

    // -------------------------------------------------------------------------



    // 게임 매니저 공용 함수 -------------------------------------------------------

    protected override void OnInitialize()
    {
        // 보드 초기화
        board = FindAnyObjectByType<Board>();
        board.Initialize(boardWidth, boardHeight, mineCount);

        FlagCount = mineCount; // 깃발 개수 설정
    }

#if UNITY_EDITOR
    public void Test_SetFlagCount(int flagCount)
    {
        FlagCount = flagCount;
    }

    public void Test_StateChange(GameState state)
    {
        State = state;
    }
#endif
}
