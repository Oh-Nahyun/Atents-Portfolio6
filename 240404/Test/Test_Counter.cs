using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Counter : TestBase
{
    public int flagCount = 5;

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        // GameManager.Instance.onFlagCountChange(flagCount);
        GameManager.Instance.Test_SetFlagCount(flagCount);
    }
}
