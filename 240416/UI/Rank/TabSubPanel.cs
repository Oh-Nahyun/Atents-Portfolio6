using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabSubPanel : MonoBehaviour
{
    /// <summary>
    /// 랭킹 한 줄을 모두 모아놓은 배열
    /// </summary>
    RankLine[] rankLines;

    /// <summary>
    /// 랭크의 종류
    /// </summary>
    public enum RankType
    {
        Action = 0, // 행동
        Time        // 시간
    }

    /// <summary>
    /// 이 패널의 타입
    /// </summary>
    public RankType rankType = RankType.Action;

    private void Awake()
    {
        rankLines = GetComponentsInChildren<RankLine>();
    }

    private void Start()
    {
        // 적절한 타이밍에 Refresh 함수 실행되게 만들기
        GameManager.Instance.onGameClear += Refresh;    // 개암 클리어 될 때 리프래쉬
        Refresh();                                      // 시작할 때 리프래쉬해서 초기화
    }

    /// <summary>
    /// RankLine들을 갱신하는 함수
    /// </summary>
    void Refresh()
    {
        // rankLines를 RankDataManager에 있는 데이터를 기반으로 갱신
        RankDataManager dataManager = GameManager.Instance.RankDataManager;

        int index = 0;
        switch (rankType) // 랭크 종류별로
        {
            case RankType.Action:
                foreach (var data in dataManager.ActionRank)
                {
                    rankLines[index].SetData<int>(index + 1, data.Data, data.Name); // 순서대로 데이터 입력
                    index++;
                }
                break;
            case RankType.Time:
                foreach (var data in dataManager.TimeRank)
                {
                    rankLines[index].SetData<float>(index + 1, data.Data, data.Name);
                    index++;
                }
                break;
        }

        // 필요없는 부분은 안보이게 만들기
        for (int i = index; i < rankLines.Length; i++)
        {
            rankLines[i].ClearLine();
        }
    }
}

/// 실습_240416
/// TabSubPanel 클래스 구현하기
