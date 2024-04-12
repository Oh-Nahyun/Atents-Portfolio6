using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Ranking : TestBase
{
    public int rank = 1;
    public int actionRecord = 15;
    public float timeRecord = 1.252f;
    public string textName = "테스트";

    public RankLine line1;
    public RankLine line2;

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        line1.SetData<int>(rank, actionRecord, textName);
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        line2.SetData<float>(rank, timeRecord, textName);
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        line1.ClearLine();
        line2.ClearLine();
    }
}

/// 실습_2404012
/// 1. RankPanel 클래스
/// 2. Tab 클래스
