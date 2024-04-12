using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_GameReset : TestBase
{
    protected override void OnTest1(InputAction.CallbackContext context)
    {
        GameManager.Instance.Test_StateChange(GameManager.GameState.GameClear);
    }
}

/// 실습_240409
/// GameClear 구현하기
/// (1) 깃발이 다 설치되어있고,
/// (2) 지뢰가 아닌 셀이 모두 열려 있어야 한다.
