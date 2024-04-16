using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 랭킹 하나에 대한 정보를 저장하는 클래스
/// </summary>
/// <typeparam name="T">랭킹 기준이 되는 정보용 데이터 타입 (IComparable 상속받아 비교가 가능한 타입이어야 한다.)</typeparam>
public class RankData<T> : IComparable<RankData<T>> where T : IComparable<T>
{
    readonly T data;
    public T Data => data;

    readonly string name;
    public string Name => name;

    public RankData(T data, string name)
    {
        this.data = data;
        this.name = name;
    }

    public int CompareTo(RankData<T> other)
    {
        //if (data.GetType() == typeof(int))
        //{
        //    return other.data.CompareTo(data);
        //}
        //else if (data.GetType() == typeof(float))
        //{
        //    return data.CompareTo(other.data);
        //}

        return data.CompareTo(other.data); // 숫자가 작은 것이 앞, 큰 것이 뒤
    }
}

/// 실습_240412
/// RankData 클래스 구현하기
/// - 리스트에 넣었을 때 정렬이 되어야 한다.
/// - T 타입은 반드시 IComparable을 상속 받은 것만 가능하다.

[Serializable]
public class RankSaveData
{
    public int[] actionCountRank;           // 1등 ~ 10등까지의 행동 횟수 저장용 배열
    public string[] actionCountRankerName;  // 1등 ~ 10등까지의 이름 (행동 횟수 기준)
    public float[] playTimeRank;            // 1등 ~ 10등까지의 플레이 시간 저장용 배열
    public string[] playTimeRankerName;     // 1등 ~ 10등까지의 이름 (플레이 시간 기준)
}

public class RankDataManager : MonoBehaviour
{
    /// <summary>
    /// 최대 표현 가능한 랭킹 개수
    /// </summary>
    const int RankCount = 10;

    /// <summary>
    /// 랭킹 정보 (행동 순위)
    /// </summary>
    List<RankData<int>> actionRank;
    public List<RankData<int>> ActionRank => actionRank;

    /// <summary>
    /// 랭킹 정보 (시간 순위)
    /// </summary>
    List<RankData<float>> timeRank;
    public List<RankData<float>> TimeRank => timeRank;

    // 상수들
    const string RankDataFolder = "Save";           // 세이브 폴더 이름
    const string RankDataFileName = "Ranking.json"; // 세이브 파일 이름 (확장자 포함)

    private void Awake()
    {
        actionRank = new List<RankData<int>>(RankCount + 1); // 랭킹 10개 + 새 정보 = 11개
        timeRank = new List<RankData<float>>(RankCount + 1);
    }

    private void Start()
    {
        LoadRankData(); // 시작할 때 데이터 로딩

        // 게임 클리어 될 때 업데이트
        GameManager.Instance.onGameClear += () => UpdateData(GameManager.Instance.ActionCount,
                                                             GameManager.Instance.PlayTime,
                                                             GameManager.Instance.PlayerName);
    }

    /// <summary>
    /// 랭킹 정보 저장하는 함수
    /// </summary>
    void SaveRankData()
    {
        RankSaveData data = new RankSaveData();                     // 세이브 정보를 저장할 클래스 만들기
        data.actionCountRank = new int[actionRank.Count];
        data.actionCountRankerName = new string[actionRank.Count];
        data.playTimeRank = new float[timeRank.Count];
        data.playTimeRankerName = new string[timeRank.Count];

        int index = 0;
        foreach (var rankData in actionRank)
        {
            data.actionCountRank[index] = rankData.Data;            // 클래스에 액션 랭킹 정보 저장
            data.actionCountRankerName[index] = rankData.Name;
            index++;
        }

        index = 0;
        foreach (var rankData in timeRank)
        {
            data.playTimeRank[index] = rankData.Data;               // 클래스에 시간 랭킹 정보 저장
            data.playTimeRankerName[index] = rankData.Name;
            index++;
        }

        string json = JsonUtility.ToJson(data);                     // 클래스에 저장된 정보를 json 형식의 텍스트로 변경
        string path = $"{Application.dataPath}/{RankDataFolder}";   // 전체 경로 만들기
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);                        // Save 폴더가 없으면 만들기
        }
        string fullPath = $"{path}/ {RankDataFileName}";
        File.WriteAllText(fullPath, json);                          // 실제 파일로 저장
    }

    /// <summary>
    /// 랭킹 정보를 파일에서 로딩하는 함수
    /// </summary>
    void LoadRankData()
    {
        string path = $"{Application.dataPath}/{RankDataFolder}";
        string fullPath = $"{path}/{RankDataFileName}";

        if (Directory.Exists(path) && File.Exists(fullPath))                // 폴더가 있고, 파일도 있어야 한다.
        {
            string json = File.ReadAllText(fullPath);                       // json 형식의 텍스트 정보를 파일에서 읽기
            RankSaveData data = JsonUtility.FromJson<RankSaveData>(json);   // json 형식의 텍스트를 클래스로 변환
            actionRank.Clear();
            timeRank.Clear();

            int count = data.actionCountRank.Length;
            for (int i = 0; i < count; i++)
            {
                actionRank.Add(new(data.actionCountRank[i], data.actionCountRankerName[i])); // 행동 랭킹 정보 읽어서 저장
            }

            count = data.playTimeRank.Length;
            for (int i = 0; i < count; i++)
            {
                timeRank.Add(new(data.playTimeRank[i], data.playTimeRankerName[i]));         // 시간 랭킹 정보 읽어서 저장
            }
        }
    }

    /// <summary>
    /// 랭킹 갱신 시도하는 함수
    /// </summary>
    /// <param name="actionCount">새 행동 횟수</param>
    /// <param name="playTime">새 클리어 시간</param>
    /// <param name="rankerName">플레이어 이름</param>
    void UpdateData(int actionCount, float playTime, string rankerName)
    {
        // 적절한 타이밍에 실행해서 파라메터 값에 따라 랭크 갱신

        // 일단 추가하고
        actionRank.Add(new(actionCount, rankerName));
        timeRank.Add(new(playTime, rankerName));

        // 정렬한 다음
        actionRank.Sort();
        timeRank.Sort();

        // 랭크의 개수가 RankCount보다 개수가 커지면 넘친 부분 제거
        if (actionRank.Count > RankCount)
        {
            actionRank.RemoveAt(RankCount);
        }
        if (timeRank.Count > RankCount)
        {
            timeRank.RemoveAt(RankCount);
        }

        // 마무리로 저장
        SaveRankData();
    }

#if UNITY_EDITOR
    public void Test_RankSetting()
    {
        actionRank = new List<RankData<int>>(10);
        actionRank.Add(new(1, "AAA"));
        actionRank.Add(new(10, "BBB"));
        actionRank.Add(new(20, "CCC"));
        actionRank.Add(new(30, "DDD"));
        actionRank.Add(new(40, "EEE"));

        timeRank = new List<RankData<float>>(10);
        timeRank.Add(new(10.0f, "AAA"));
        timeRank.Add(new(20.0f, "BBB"));
        timeRank.Add(new(30.0f, "CCC"));
        timeRank.Add(new(40.0f, "DDD"));
        timeRank.Add(new(50.0f, "EEE"));
    }

    public void Test_ActionUpdate(int rank, string name)
    {
        actionRank.Add(new(rank, name));
        actionRank.Sort();
    }

    public void Test_TimeUpdate(float rank, string name)
    {
        timeRank.Add(new(rank, name));
        timeRank.Sort();
    }

    public void Test_Save()
    {
        SaveRankData();
    }

    public void Test_Load()
    {
        LoadRankData();
    }
#endif
}

/// 실습_240416
/// UpdateData() 구현하기
